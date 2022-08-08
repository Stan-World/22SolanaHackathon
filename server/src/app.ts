import express, { Application } from 'express';
import * as expressApp from 'express';
import cors from 'cors';
import session, { SessionOptions } from 'express-session';
import cookieParser from 'cookie-parser';
import 'dotenv/config';

declare module 'express-session' {
  interface SessionData {
    user: {
      uid: string;
    };
    fandom: {
      id: number;
    };
  }
}

export class App {
  public app: Application;
  public port: number;

  constructor(controllers, port) {
    this.app = express();
    this.port = port;

    this.initializeMiddlewares();
    this.initSession(this.app);
    this.initializeControllers(controllers);
  }

  private initializeMiddlewares() {
    this.app.use(expressApp.urlencoded({ extended: true, limit: '50mb' }));
    this.app.use(expressApp.json({ limit: '50mb' }));
  }

  private initializeControllers(controllers) {
    controllers.forEach((controller) => {
      this.app.use('/', controller.router);
    });
  }

  /**
   * Session Settings
   */
  private initSession(app: Application, env = 'dev') {
    const options: SessionOptions = {
      name: 'stan.session',
      secret: 'ab8fb#cbqvt2dc%t%8%0*hsc!r$v6',
      resave: false,
      saveUninitialized: true,
      proxy: false,
      rolling: true,
      cookie: {
        maxAge: 10_000,
      },
    };

    if (env !== 'dev') {
      app.set('trust proxy', 1);
    }

    app.use(
      cors({
        credentials: true,
        origin: '*',
      }),
    );
    app.use(cookieParser());
    app.use(session(options as SessionOptions));
    return true;
  }

  public listen() {
    return this.app.listen(this.port, () => {
      console.log('Server Started!');
    });
  }
}
