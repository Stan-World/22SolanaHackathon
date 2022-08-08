import { Memcache } from '../util';
import { User, UserFandom } from '../model';
import { DefaultFandomId, Table } from '../data';
import { SessionData } from 'express-session';

export class UserService {
  private _memcache = new Memcache();

  public async getUserList() {
    return this._memcache.getFromMemory(`${Table.user}`);
  }

  public async getUser(uid: string) {
    const user = this._getUserFromMemory(uid);
    if (user) {
      return await this._formatUser(user);
    }
    return null;
  }

  public async createUser(
    uid: string,
    name: string,
    fandomId: number = DefaultFandomId,
  ) {
    // 유저 생성
    const user = await this._formatUser(User.createUser(uid, name));
    this._memcache.setToMemory(`${Table.user}.${uid}`, user);
    // @TODO unique인지 확인 필요
    user.index =
      Object.keys(this._memcache.getFromMemory(`${Table.user}`)).length - 1;
    // 유저 - 팬덤 alias 생성
    const userFandom = UserFandom.createUserFandom(`${uid}`, fandomId);
    this._memcache.setToMemory(`${Table.userFandom}.${uid}`, userFandom);
    return {
      ...user,
      fandom: this._memcache.getFromMemory(`${Table.fandom}.${fandomId}`),
    };
  }

  public changeUserStatus(uid: string, status: boolean) {
    this._memcache.setToMemory(`${Table.user}.${uid}.online`, status);
  }

  public async getOfflineUsers(
    sessions: SessionData[] | { [sid: string]: SessionData },
  ) {
    // @TODO 추후 같은 팬덤의 유저만 가져오게 변경
    let users = Object.values(
      this._memcache.getFromMemory(`${Table.user}`),
    ).filter((user: User) => user.online) as Array<User>;

    await Promise.all(
      Object.values(sessions).map((session: { user: { uid: string } }) => {
        users = users.filter((user) => user.uid !== session.user?.uid);
      }),
    );
    return users;
  }

  public async incrementVitaPoint(uid: string, time: number, rate: number) {
    const user = this._getUserFromMemory(uid);
    return this._memcache.setToMemory(
      `${Table.user}.${uid}.vitaPoint`,
      rate / time + user.vitaPoint,
    );
  }

  public async adjustStanGem(stanGem: number) {
    const onlineUsers = Object.values(await this.getUserList()).filter(
      (user: User) => user.online === true,
    ) as Array<User>;
    const total = onlineUsers.reduce((prev: number, curr: User) => {
      prev += curr.vitaPoint;
      return prev;
    }, 0);
    const allUsers = onlineUsers.map((user: User) => {
      user.stanGem += Math.round(stanGem * (user.vitaPoint / total));
      this._memcache.setToMemory(`${Table.user}.${user.uid}`, user);
      return user;
    });
    return allUsers;
  }

  public async stakeStanGem(uid: string, stanGem: number) {
    const user = this._getUserFromMemory(uid);
    user.stake += stanGem;
    user.stanGem -= stanGem;
    this._memcache.setToMemory(`${Table.user}.${uid}`, user);
    return await this._formatUser(user);
  }

  public async getLightStick(uid: string) {
    return this._memcache.setToMemory(`${Table.user}.${uid}.lightStick`, true);
  }

  private _getUserFromMemory(uid: string) {
    return this._memcache.getFromMemory(`${Table.user}.${uid}`) as User;
  }

  private async _formatUser(user: User) {
    const total = Object.values(await this.getUserList()).reduce(
      (prev: number, curr: User) => (prev += curr.stake),
      0,
    ) as number;
    user.power = total > 0 ? Math.round((user.stake / total) * 100) : 0;
    this._memcache.setToMemory(`${Table.user}.${user.uid}`, user);
    return user;
  }
}
