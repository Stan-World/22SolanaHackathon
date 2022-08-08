import { Party, Streaming } from '../model';

export const StremaingList: Array<Streaming> = [
  {
    url: 'https://www.youtube.com/watch?v=Hbb5GPxXF1w',
    runTime: 184,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=Y8JFxS1HlDo',
    runTime: 178,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=f6YDKF0LVWw',
    runTime: 170,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=dYRITmpFbJ4',
    runTime: 268,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=gdZLi9oWNZg',
    runTime: 223,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=js1CtxSY38I',
    runTime: 262,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=LWRwlPQNZnk',
    runTime: 232,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=eDEFolvLn0A',
    runTime: 217,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=7tkbzxa8MFQ',
    runTime: 183,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=R9At2ICm4LQ',
    runTime: 237,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=VCDWg0ljbFQ',
    runTime: 216,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=Jh4QFaPmdss',
    runTime: 197,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=lDV5cM9YE4g',
    runTime: 190,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=uBY1AoiF5Vo',
    runTime: 230,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=hr-325mclek',
    runTime: 198,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=aiHSVQy9xN8',
    runTime: 207,
    viewCount: 0,
  },
  {
    url: 'https://www.youtube.com/watch?v=HPQ5mqovXHo',
    runTime: 312,
    viewCount: 0,
  },
];

export const Parties: { [key: number]: Party } = {
  1: {
    id: 1,
    streamingList: StremaingList,
    streamingIndex: 0,
    currentTime: 0,
    createdDate: new Date('2022-07-01'),
    endDate: null,
  },
};
