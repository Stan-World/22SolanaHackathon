import { CronJob } from 'cron';
import axios from 'axios';
import { User } from './model';

const baseUrl = 'http://localhost:4000';

const setOfflineUsers = async () => {
  const setOfflinePath = 'login/set-offline';
  try {
    const { data } = await axios.post(`${baseUrl}/${setOfflinePath}`, {
      cmd: setOfflinePath,
    });
    if (data.offlineUsers.length > 0) {
      console.log(
        'Offline User: ' +
          data.offlineUsers.map((user: User) => user.uid).toString(),
      );
    }
  } catch (e) {
    console.log('Error from setOffline' + e.message);
  }
};

const incrementVitaPoint = async (time = 1) => {
  const incrementVitaPointPath = 'user/vp';
  try {
    const usersData = await axios.post(`${baseUrl}/user`);
    const users = Object.values(usersData.data);
    await Promise.all(
      users
        .filter((user: User) => user.online === true)
        .map(async (user: User) => {
          const { data } = await axios.post(
            `${baseUrl}/${incrementVitaPointPath}`,
            {
              uid: user.uid,
              time: 1 / time,
            },
          );
          if (data.vitaPoint) {
            console.log(data.vitaPoint);
            return data;
          }
        }),
    );
  } catch (e) {
    console.log('Error from incrementVitaPoint' + e.message);
  }
};

const adjustVitaPoint = async () => {
  const adjustVitaPointPath = 'fandom/adjust';
  try {
    await axios.post(`${baseUrl}/${adjustVitaPointPath}`, {
      cmd: adjustVitaPointPath,
    });
  } catch (e) {
    console.log('Error from adjustVitaPoint' + e.message);
  }
};

const calcViewCount = async () => {
  const calcViewCountPath = 'fandom/view-count';
  try {
    await axios.post(`${baseUrl}/${calcViewCountPath}`, {
      cmd: calcViewCountPath,
    });
  } catch (e) {
    console.log('Error from calcViewCount' + e.message);
  }
};

const minuteCron = new CronJob('* * * * * *', async () => {
  // Set offline all users w/o session
  await setOfflineUsers();
  // calculate view count for each video
  await calcViewCount();
});

const minute2Cron = new CronJob('*/2 * * * * *', async () => {
  // set Vita Point every 2sec for all online users
  await incrementVitaPoint(2);
  // adjust Vita Point for online users
  await adjustVitaPoint();
});

const main = () => {
  console.log('Cron server started');
  // 1분짜리 크론 시작
  minuteCron.start();
  // 2분짜리 크론 시작
  minute2Cron.start();
};

main();
