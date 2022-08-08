import { Request } from 'express';

export class Result {
  public static createResult(req: Request, data: any) {
    const { method, body, originalUrl } = req;
    return {
      cmd: method === 'GET' ? originalUrl.replace('/', '') : body.cmd,
      ...data,
    };
  }
}
