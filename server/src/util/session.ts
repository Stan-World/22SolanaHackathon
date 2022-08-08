import { NextFunction, Request, Response } from 'express';
import { DefaultFandomId } from '../data';
import { Error } from './error';

export function checkSession(req: Request, res: Response, next: NextFunction) {
  const { session } = req;
  if (!session.user || !session.fandom) {
    res.status(401).send(Error.createError(req, 'Please Log In Again!'));
    return;
  }
  next();
}

export function setSession(req: Request, uid: string) {
  /**
   * 세션에 유저와 팬덤 업데이트
   */
  const { session } = req;
  session.user = {
    uid,
  };
  session.fandom = {
    id: DefaultFandomId,
  };
}

export function deleteSession(req: Request) {
  const { session } = req;
  return new Promise((resolve, reject) =>
    session.destroy((err) => {
      if (err) {
        reject(err);
      }
      resolve(true);
    }),
  );
}
