import { App } from './app';
import { Router } from './controllers';

try {
  const app = new App([new Router()], 4000);

  app.listen();
} catch (e) {
  console.log(e);
}
