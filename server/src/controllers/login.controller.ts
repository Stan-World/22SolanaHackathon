import { Request, Response } from 'express';
import * as express from 'express';
import { FandomService, UserService, SolanaService } from '../services';
import { deleteSession, Error, Result, setSession } from '../util';
import { Table } from '../data';

export class LoginController {
  private _userService = new UserService();
  private _fandomService = new FandomService();
  private _solanaService = new SolanaService();
  public router = express.Router();

  constructor() {
    this.initRoutes();
  }

  public initRoutes() {
    this.router.post('/set-offline', this._setUsersOffline);
    this.router.post('/off', this._logout);
    this.router.post('/', this._login);
  }

  private _login = async (req: Request, res: Response) => {
    const { body } = req;
    try {
      const user = await this._userService.getUser(body.uid);
      if (user) {
        const userFandom = await this._fandomService.getUserFandom(user.uid);
        const fandom = await this._fandomService.getFandom(userFandom.fandomId);
        this._userService.changeUserStatus(user.uid, true);
        setSession(req, body.uid);
        res.send(Result.createResult(req, { ...user, fandom }));
        return;
      }
      const result = await this._userService.createUser(body.uid, body.name);
      setSession(req, body.uid);
      res.send(Result.createResult(req, result));
      // 비동기 처리: function으로 wrap해서 await 하지 않게 처리
      this._solanaService.createKey(result.uid, Table.wallet);
      return;
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _setUsersOffline = async (req: Request, res: Response) => {
    const { sessionStore } = req;
    const data = await new Promise((resolve, reject) => {
      sessionStore.all(async (e, sessions) => {
        if (e) {
          reject(e);
        }
        const inactiveUsers = await this._userService.getOfflineUsers(sessions);
        // @TODO 현재는 메모리에 들고있기때문에 하나씩 변경하지만 추후 RDB를 쓰게 되면 한꺼번에 변경이 가능합니다.
        inactiveUsers.forEach((user) =>
          this._userService.changeUserStatus(user.uid, false),
        );
        resolve(inactiveUsers);
      });
    });

    res.send(Result.createResult(req, { offlineUsers: data }));
  };

  private _logout = async (req: Request, res: Response) => {
    const { body } = req;
    try {
      const user = await this._userService.getUser(body.uid);
      if (!user) {
        res.status(404).send(Error.createError(req, 'User not found.'));
        return;
      }
      this._userService.changeUserStatus(user.uid, false);
      await deleteSession(req);
      res.send(Result.createResult(req, { online: false }));
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };
}
