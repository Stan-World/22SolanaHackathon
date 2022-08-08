import { Request } from 'express';

export class Error {
  cmd: string;
  message: string;

  public static createError(req: Request, message: string) {
    const { method, body, originalUrl } = req;
    const error = new Error();
    error.cmd = method === 'GET' ? originalUrl.replace('/', '') : body.cmd;
    error.message = message;
    return error;
  }
}
