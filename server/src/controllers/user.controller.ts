import { Request, Response } from 'express';
import * as express from 'express';
import { FandomService, SolanaService, UserService } from '../services';
import { Result, Error, checkSession } from '../util';
import { DefaultFandomId } from '../data';

export class UserController {
  private _userService = new UserService();
  private _fandomService = new FandomService();
  private _solanaService = new SolanaService();
  public router = express.Router();

  constructor() {
    this.initRoutes();
  }

  public initRoutes() {
    this.router.post('/vp', this._incVitaPoint);
    this.router.post('/light-stick', checkSession, this._getLightStick);
    this.router.post('/use-gem', checkSession, this._useStanGem);
    this.router.post('/', this._getAllUsers);
  }

  private _incVitaPoint = async (req: Request, res: Response) => {
    const { body } = req;
    try {
      const fandomLevel = await this._fandomService.getFandomLevel(
        DefaultFandomId,
      );
      const vp = await this._userService.incrementVitaPoint(
        body.uid,
        fandomLevel.time * (body.time ?? 1),
        fandomLevel.vitaPointRate,
      );
      res.send(Result.createResult(req, { vp }));
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _getAllUsers = async (req: Request, res: Response) => {
    const users = await this._userService.getUserList();
    res.send(Result.createResult(req, users));
  };

  private _getLightStick = async (req: Request, res: Response) => {
    const { session } = req;
    const lightStick = await this._userService.getLightStick(session.user.uid);
    res.send(Result.createResult(req, { lightStick }));
  };

  private _useStanGem = async (req: Request, res: Response) => {
    const { session, body } = req;
    try {
      const user = await this._userService.stakeStanGem(
        session.user.uid,
        +body.amount,
      );
      res.send(Result.createResult(req, user));
      // 비동기 처리: function으로 wrap해서 await 하지 않게 처리
      this._solanaService.burnTokens(user.uid, +body.amount);
      return;
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };
}
