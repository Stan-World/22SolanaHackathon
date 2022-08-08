import { Request, Response } from 'express';
import * as express from 'express';
import { checkSession } from '../util/session';
import { Result } from '../util';

export class PingController {
  public router = express.Router();

  constructor() {
    this.initRoutes();
  }

  public initRoutes() {
    this.router.post('/', checkSession, this._ping);
  }

  private _ping = async (req: Request, res: Response) => {
    res.send(Result.createResult(req, true));
  };
}
