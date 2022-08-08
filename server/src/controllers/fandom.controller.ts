import { Request, Response } from 'express';
import * as express from 'express';
import { checkSession, Result, Error } from '../util';
import { FandomService, SolanaService, UserService } from '../services';
import { UserFandom } from '../model';
import { DefaultFandomId } from '../data';

export class FandomController {
  public router = express.Router();
  private _fandomService = new FandomService();
  private _userService = new UserService();
  private _solanaService = new SolanaService();

  constructor() {
    this.initRoutes();
  }

  public initRoutes() {
    this.router.post('/levels', this._getFandomLevels);
    this.router.post('/shop', this._getFandomShop);
    this.router.post('/', checkSession, this._getFandom);
    this.router.post('/adjust', this._adjustVitaPoint);
    this.router.post('/stake', checkSession, this._stakeStanGem);
    this.router.post('/view-count', this._calcViewCount);
    this.router.post('/purchase', checkSession, this._purchaseItem);
  }

  private _getFandom = async (req: Request, res: Response) => {
    const { session } = req;

    try {
      const fandom = await this._fandomService.getFandom(session.fandom.id);
      const allUserFandoms = await this._fandomService.getAllUsersInFandom(
        fandom.id,
      );
      const allUsers = await this._userService.getUserList();
      const party = await this._fandomService.getParty();
      const items = await this._fandomService.getFandomItems();
      res.send(
        Result.createResult(req, {
          ...fandom,
          users: allUserFandoms.map((userFandom: UserFandom) => {
            return allUsers[userFandom.uid];
          }),
          party,
          items,
        }),
      );
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _adjustVitaPoint = async (req: Request, res: Response) => {
    const { body } = req;
    const fandomId = body.id ?? DefaultFandomId;
    try {
      const fandomLevel = await this._fandomService.getFandomLevel(fandomId);
      const fandom = await this._fandomService.adjustVitaPoint(
        fandomId,
        fandomLevel.vitaPointRatio,
        fandomLevel.vitaGoldRatio,
      );
      if (fandom.unit > 0) {
        const allUsers = await this._userService.adjustStanGem(
          fandom.unit * fandomLevel.stanGemRatio,
        );
        // 비동기 처리: function으로 wrap해서 await 하지 않게 처리
        this._solanaService.adjustTokens(
          allUsers,
          fandom.unit * fandomLevel.stanGemRatio,
        );
      }
      res.send(Result.createResult(req, fandom));
      return;
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _stakeStanGem = async (req: Request, res: Response) => {
    const { body, session } = req;
    try {
      const fandom = await this._fandomService.stakeStanGem(
        session.fandom.id,
        +body.amount,
      );
      const user = await this._userService.stakeStanGem(
        session.user.uid,
        +body.amount,
      );
      res.send(Result.createResult(req, { ...user, fandom }));
      // 비동기 처리: function으로 wrap해서 await 하지 않게 처리
      this._solanaService.initStake(user, +body.amount);
      return;
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _calcViewCount = async (req: Request, res: Response) => {
    try {
      const party = await this._fandomService.getParty();
      if (party.currentTime === 0) {
        party.streamingList = this._fandomService.incViewCount(
          party.streamingList,
          party.streamingIndex,
        );
      }
      res.send(Result.createResult(req, party));
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _getFandomLevels = async (req: Request, res: Response) => {
    try {
      const fandomLevels = this._fandomService.getFandomLevelTable();
      res.send(Result.createResult(req, { table: fandomLevels }));
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _getFandomShop = async (req: Request, res: Response) => {
    try {
      const shop = this._fandomService.getFandomShop();
      res.send(Result.createResult(req, { shop }));
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };

  private _purchaseItem = async (req: Request, res: Response) => {
    try {
      const { body, session } = req;
      const item = this._fandomService.getShopItem(body.itemId);
      if (!item) {
        res.status(404).send(Error.createError(req, 'Shop item not found'));
        return;
      }
      const items = await this._fandomService.purchaseItem(
        item,
        session.fandom.id,
      );
      res.send(Result.createResult(req, { items }));
    } catch (e) {
      res.status(400).send(Error.createError(req, e.message));
    }
  };
}
