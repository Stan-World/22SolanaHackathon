import { PublicKey } from '@solana/web3.js';

export enum TransactionType {
  transfer = 'transfer',
  stake = 'stake',
  burn = 'burn',
}

export class Wallet {
  uid: string;
  key: { publicKey: string; secretKey: string };
  tokenAccount: PublicKey;
  createdDate: Date;

  public static create(
    uid: string,
    key: { publicKey: string; secretKey: string },
    tokenAccount: PublicKey,
  ) {
    const wallet = new Wallet();
    wallet.uid = uid;
    wallet.key = key;
    wallet.tokenAccount = tokenAccount;
    wallet.createdDate = new Date();
    return wallet;
  }
}

export class TransactionHist {
  uid: string;
  transactionId: string;
  type: TransactionType;
  createdDate: Date;

  public static create(
    uid: string,
    transactionId: string,
    type: TransactionType,
  ) {
    const trhist = new TransactionHist();
    trhist.uid = uid;
    trhist.transactionId = transactionId;
    trhist.type = type;
    trhist.createdDate = new Date();
    return trhist;
  }
}

export class Stake {
  uid: string;
  stakeId: string;
  amount: number;
  createdDate: Date;

  public static create(uid: string, stakeId: string, amount: number) {
    const stake = new Stake();
    stake.uid = uid;
    stake.stakeId = stakeId;
    stake.amount = amount;
    stake.createdDate = new Date();
    return stake;
  }
}
