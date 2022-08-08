import { DefaultFandomId } from '../data';

export class UserFandom {
  uid: string;
  fandomId: number;
  createdDate: Date;

  public static createUserFandom(
    uid: string,
    fandomId: number = DefaultFandomId,
  ) {
    const userFandom = new UserFandom();
    userFandom.uid = uid;
    userFandom.fandomId = fandomId;
    userFandom.createdDate = new Date();
    return userFandom;
  }
}

export class Fandom {
  id: number;
  name: string;
  adjust: number;
  vitaPoint: number;
  vitaGold: number;
  stanGem: number;
  level: number;
  createdDate: Date;
}

export class FandomLevel {
  level: number;
  minGem: number;
  maxGem: number;
  vitaPointRatio: number;
  vitaGoldRatio: number;
  stanGemRatio: number;
  time: number;
  vitaPointRate: number;
}
