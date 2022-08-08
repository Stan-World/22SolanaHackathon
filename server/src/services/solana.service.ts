import { Memcache } from '../util';
import { clusterApiUrl, Connection, Keypair, PublicKey } from '@solana/web3.js';
import { Table } from '../data';
import { AES } from 'crypto-js';
import * as CryptoJS from 'crypto-js';
import { Wallet, TransactionHist, TransactionType, User } from '../model';
import {
  transferChecked,
  createAccount,
  getAccount,
  approveChecked,
  burnChecked,
} from '@solana/spl-token';

export class SolanaService {
  private _memcache = new Memcache();
  private _connection = new Connection(clusterApiUrl('devnet'), 'confirmed');
  private _stanWorldKey: Keypair;
  private _feePayer: Keypair;
  private _mint: PublicKey;
  private _ata: PublicKey;

  constructor() {
    this._stanWorldKey = new Keypair({
      publicKey: new Uint8Array(JSON.parse(`[${process.env['STAN_PUB_KEY']}]`)),
      secretKey: new Uint8Array(JSON.parse(`[${process.env['STAN_PRV_KEY']}]`)),
    });
    this._feePayer = new Keypair({
      publicKey: new Uint8Array(JSON.parse(`[${process.env['FEE_PUB_KEY']}]`)),
      secretKey: new Uint8Array(JSON.parse(`[${process.env['FEE_PRV_KEY']}]`)),
    });
    this._mint = new PublicKey(process.env['STAN_MINT_PUB']);
    this._ata = new PublicKey(process.env['STAN_ATA_PUB']);
  }

  public async createKey(uid: string, table: Table) {
    const keyPair = Keypair.generate();
    const pair = {
      publicKey: keyPair.publicKey.toString(),
      secretKey: this._encrypt(keyPair.secretKey.toString()),
    };
    const tokenAccount = await this._creatTokenAccount(keyPair);
    const key = Wallet.create(uid, pair, tokenAccount);
    this._memcache.setToMemory(`${table}.${uid}`, key);
    return { key: keyPair, tokenAccount };
  }

  public async getKey(uid: string) {
    const wallet = this._memcache.getFromMemory(
      `${Table.wallet}.${uid}`,
    ) as Wallet;
    if (wallet) {
      return {
        key: new Keypair({
          publicKey: new PublicKey(wallet.key.publicKey).toBuffer(),
          secretKey: new Uint8Array(
            JSON.parse(`[${this._decrypt(wallet.key.secretKey)}]`),
          ),
        }),
        tokenAccount: wallet.tokenAccount,
      };
    }
    return null;
  }

  public async stake(
    uid: string,
    from: { key: Keypair; tokenAccount: PublicKey },
    amount: number,
  ) {
    const stake = await this.transferChecked(
      uid,
      this._ata,
      amount,
      from.tokenAccount,
      from.key,
      TransactionType.stake,
    );
    await this._delegateToken(from, amount);
    return stake;
  }

  public async transferChecked(
    uid: string,
    destination: PublicKey,
    amount: number,
    source: PublicKey = this._ata,
    owner: Keypair = this._stanWorldKey,
    transferType: TransactionType = TransactionType.transfer,
  ) {
    try {
      const trHash = await transferChecked(
        this._connection,
        this._feePayer,
        source,
        this._mint,
        destination,
        owner,
        amount,
        8,
      );
      this._memcache.setToMemory(
        `${Table.transaction}.${
          this._getHistoryList(Table.transaction).length
        }`,
        TransactionHist.create(uid, trHash, transferType),
      );
      await this._getBalance(destination, `Transfer type ${transferType}`);
      return trHash;
    } catch (e) {
      throw new Error(e.message);
    }
  }

  public async initStake(user: User, amount: number) {
    const from = await this.getKey(user.uid);
    const result = await this.stake(user.uid, from, amount);
    return result;
  }

  public async burnTokens(uid: string, amount: number) {
    const from = await this.getKey(uid);
    const trHash = await burnChecked(
      this._connection,
      this._feePayer,
      from.tokenAccount,
      this._mint,
      from.key,
      amount,
      8,
    );
    this._memcache.setToMemory(
      `${Table.transaction}.${this._getHistoryList(Table.transaction).length}`,
      TransactionHist.create(uid, trHash, TransactionType.burn),
    );
    await this._getBalance(from.tokenAccount, uid);
    return trHash;
  }

  public async adjustTokens(userList: Array<User>, amount: number) {
    const results = Promise.all(
      userList.map(async (user) => {
        const key = await this.getKey(user.uid);
        if (!key) {
          throw new Error(`User's token account not found.`);
        }
        const result = await this.transferChecked(
          user.uid,
          key.tokenAccount,
          amount,
        );
        return result;
      }),
    );
    return results;
  }

  private async _creatTokenAccount(to: Keypair) {
    const account = await createAccount(
      this._connection,
      this._feePayer,
      this._mint,
      to.publicKey,
    );
    return account;
  }

  private async _getBalance(publicKey: PublicKey, account: string) {
    const tokenAccount = await getAccount(this._connection, publicKey);
    const total = await getAccount(this._connection, this._ata);
    console.log(`${account} has ${tokenAccount.amount} tokens.`);
    console.log(`Remaining tokens: ${total.amount}.`);
    return tokenAccount.amount;
  }

  private _encrypt(message: string) {
    return AES.encrypt(message, process.env['ENCRYPTION_KEY']).toString();
  }

  private _decrypt(message: string) {
    return AES.decrypt(message, process.env['ENCRYPTION_KEY']).toString(
      CryptoJS.enc.Utf8,
    );
  }

  private _getHistoryList(table: Table) {
    return this._memcache.getFromMemory(`${table}`) as Array<TransactionHist>;
  }
  private async _delegateToken(
    from: { key: Keypair; tokenAccount: PublicKey },
    amount: number,
  ) {
    const trHash = await approveChecked(
      this._connection,
      this._feePayer,
      this._mint,
      from.tokenAccount,
      this._ata,
      from.key,
      amount,
      8,
    );
    await this._getBalance(from.tokenAccount, 'Delegator');
    return trHash;
  }
}
