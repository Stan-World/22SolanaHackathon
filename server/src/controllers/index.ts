import * as express from 'express';
import { Request, Response } from 'express';
import { LoginController } from './login.controller';
import { PingController } from './ping.controller';
import { FandomController } from './fandom.controller';
import { UserController } from './user.controller';

export class Router {
  private _login = new LoginController();
  private _fandom = new FandomController();
  private _ping = new PingController();
  private _user = new UserController();
  public router = express.Router();

  constructor() {
    this.initRouter();
  }

  public initRouter() {
    /**
     * Simple Health Check
     */
    this.router.get('/', (req: Request, res: Response) => {
      res.send('Health check successful!');
    });

    /**
     * Routing Table
     */
    this.router.use('/login', this._login.router);
    this.router.use('/fandom', this._fandom.router);
    this.router.use('/ping', this._ping.router);
    this.router.use('/user', this._user.router);
  }
}
