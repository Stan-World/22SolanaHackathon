import { Memcache } from '../util';
import { DefaultFandomId, Table } from '../data';
import {
  Fandom,
  FandomLevel,
  Party,
  Streaming,
  User,
  UserFandom,
  Item,
} from '../model';

export class FandomService {
  private _memcache = new Memcache();

  public async getUserFandom(uid: string) {
    return this._memcache.getFromMemory(
      `${Table.userFandom}.${uid}`,
    ) as UserFandom;
  }

  public async getFandom(id: number) {
    const fandom = this._getFandomFromMemory(id);
    return this._formatFandom(fandom);
  }

  public async getAllUsersInFandom(id: number = DefaultFandomId) {
    const allUsersInFandoms = this._memcache.getFromMemory(
      `${Table.userFandom}`,
    );
    return Object.values(allUsersInFandoms).filter(
      (userFandom: UserFandom) => userFandom.fandomId === id,
    ) as Array<UserFandom>;
  }

  public async getParty(id: number = DefaultFandomId) {
    const party = this._memcache.getFromMemory(`${Table.party}.${id}`) as Party;
    const streamingTime = Math.ceil(
      (new Date().getTime() - party.createdDate.getTime()) / 1000,
    );
    const current = this._getCurrentStreaming(
      party.streamingList,
      streamingTime,
    );
    party.streamingIndex = current.index;
    party.currentTime = current.currTime;
    return party;
  }

  public async getFandomLevel(id: number) {
    const fandom = this._getFandomFromMemory(id);
    const fandomLevel = this._memcache.getFromMemory(
      `${Table.fandomLevel}`,
    ) as Array<FandomLevel>;
    return fandomLevel.find((level) => level.level === fandom.level);
  }

  public async adjustVitaPoint(id: number, vpRatio: number, vgRatio: number) {
    const fandom = await this.getFandom(id);
    const unit = Math.floor((fandom.vitaPoint - fandom.adjust) / vpRatio);
    if (unit > 0) {
      fandom.adjust += unit * vpRatio;
      fandom.vitaGold += unit * vgRatio;
      this._memcache.setToMemory(`${Table.fandom}.${id}`, fandom);
    }
    return { ...fandom, unit };
  }

  public async stakeStanGem(id: number, stanGem: number) {
    const fandom = this._getFandomFromMemory(id);
    fandom.stanGem += stanGem;
    this._memcache.setToMemory(`${Table.fandom}.${id}`, fandom);
    return this._formatFandom(fandom);
  }

  public incViewCount(streamingList: Array<Streaming>, index: number) {
    const prevIndex = !index ? streamingList.length - 1 : index - 1;
    const views = Object.values(
      this._memcache.getFromMemory(`${Table.user}`),
    ).filter((user: User) => user.online === true).length;
    streamingList[prevIndex].viewCount += views;
    this._memcache.setToMemory(
      `${Table.party}.${DefaultFandomId}.streamingList`,
      streamingList,
    );
    return streamingList;
  }

  public getFandomLevelTable() {
    return this._memcache.getFromMemory(
      `${Table.fandomLevel}`,
    ) as Array<FandomLevel>;
  }

  public getFandomShop() {
    return this._memcache.getFromMemory(`${Table.shop}`) as Array<Item>;
  }

  public getShopItem(id: string) {
    const items = this.getFandomShop();
    return items.find((item) => item.id === id);
  }

  public getFandomItems() {
    return this._memcache.getFromMemory(`${Table.fandomItems}`) as Array<Item>;
  }

  public async purchaseItem(item: Item, fandomId = DefaultFandomId) {
    const fandom = this._getFandomFromMemory(fandomId);
    if (fandom.vitaGold < item.price) {
      throw new Error(
        `No sufficient funds. Your fandom has ${fandom.vitaGold} vita golds but the item costs ${item.price} vita golds.`,
      );
    }
    const items = this.getFandomItems();
    items.push(item);
    this._memcache.setToMemory(`${Table.fandomItems}`, items);
    this._memcache.setToMemory(
      `${Table.fandom}.${fandomId}.vitaGold`,
      fandom.vitaGold - item.price,
    );
    return items;
  }

  private _getCurrentStreaming(
    streamings: Array<Streaming>,
    streamingTime: number,
  ) {
    const totalRunTime = streamings.reduce(
      (prev, curr) => (prev += curr.runTime),
      0,
    );
    let remainder = streamingTime % totalRunTime;
    return streamings.reduce(
      (prev: { index: number; currTime: number }, curr: Streaming) => {
        if (remainder <= 0) {
          return prev;
        } else if (remainder - curr.runTime < 0) {
          prev.currTime = remainder;
          remainder -= curr.runTime;
          return prev;
        }
        prev.index++;
        remainder -= curr.runTime;
        return prev;
      },
      { index: 0, currTime: 0 },
    );
  }

  private _getFandomFromMemory(id: number) {
    return this._memcache.getFromMemory(`${Table.fandom}.${id}`) as Fandom;
  }

  private async _formatFandom(fandom: Fandom) {
    const fandomLevel = this._calcFandomLevel(fandom.stanGem);
    fandom.level = fandomLevel ?? 0;
    const vp = await this._calcFandomVitaPoint(fandom.id);
    fandom.vitaPoint = vp;
    return this._memcache.setToMemory(
      `${Table.fandom}.${fandom.id}`,
      fandom,
    ) as Fandom;
  }

  private _calcFandomLevel(stanGem: number) {
    const fandomLevels = this.getFandomLevelTable();
    return fandomLevels.find(
      (level) =>
        stanGem >= level.minGem &&
        (level.maxGem < 0 || stanGem <= level.maxGem),
    ).level;
  }

  private async _calcFandomVitaPoint(id: number) {
    // @TODO 팬덤별로 필터링 하기
    const allUsers = await this._memcache.getFromMemory(`${Table.user}`);
    const users = (await this.getAllUsersInFandom(id)).map(
      (userFandom) => allUsers[userFandom.uid],
    ) as Array<User>;
    return users.reduce((prev, curr) => {
      prev += curr.vitaPoint;
      return prev;
    }, 0);
  }
}
