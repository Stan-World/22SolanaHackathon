export class User {
  uid: string;
  name: string;
  online: boolean;
  stanGem: number;
  power: number;
  index: number;
  stake: number;
  lightStick: boolean;
  vitaPoint: number;
  createdDate: Date;

  public static createUser(uid: string, name: string) {
    const user = new User();
    user.uid = uid;
    user.name = name;
    user.online = true;
    user.stanGem = 0;
    user.stake = 0;
    user.lightStick = false;
    user.power = 0;
    user.vitaPoint = 0;
    user.createdDate = new Date();
    return user;
  }
}
