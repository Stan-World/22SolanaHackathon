# 2022 Solana Summer Camp Online Hackathon

## Description
### Server
This main server provides API endpoints for clients to use.

### Scheduler
Scheduler serves to increment and adjust vitapoints. Must run on the same server as main server.

## Preparation
1. Install Node.js
2. Install Yarn
3. Install Eslint, TypeScript, 
4. Replace values at `.env` to valid input.
    * var ending in _PUB_KEY or _PRV_KEY: 123,12,23,123,....
    * var ending in _PUB: DbjsUte2d....

## Build and Run
1. Install dependencies
```
> yarn install
```
2. Build TypeScript
```
> tsc
```
3. Run server using Node
```
> node dist/main.js
```
4. Run scheduler using Node
```
> node dist/main.cron.js
```