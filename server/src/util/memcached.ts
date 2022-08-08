import { Fandoms, Table, Parties, FandomLevels, Items } from '../data';

export class Memcache {
  private _memory = {};

  constructor() {
    this._loadData();
    global._memory = this._memory;
  }

  public getFromMemory = (key: string) => {
    return key.split('.').reduce((obj, path) => obj?.[path], global._memory);
  };

  public setToMemory = (key: string, data: any) => {
    key
      .split('.')
      .reduce(
        (obj, path, idx) =>
          (obj[path] =
            key.split('.').length === ++idx
              ? data
              : obj[path] || global._memory),
        global._memory,
      );
    return this.getFromMemory(key);
  };

  /**
   * Private Methods
   */
  private _loadData() {
    this._memory[Table.fandom] = Fandoms;
    this._memory[Table.fandomLevel] = FandomLevels;
    this._memory[Table.user] = {};
    this._memory[Table.wallet] = {};
    this._memory[Table.transaction] = [];
    this._memory[Table.userFandom] = {};
    this._memory[Table.party] = Parties;
    this._memory[Table.shop] = Items;
    this._memory[Table.fandomItems] = [];
  }
}
