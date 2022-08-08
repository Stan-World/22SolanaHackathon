export class Streaming {
  url: string;
  runTime: number;
  viewCount: number;
}

export class Party {
  id: number;
  streamingList: Array<Streaming>;
  streamingIndex: number;
  currentTime: number;
  createdDate: Date;
  endDate: Date | null;
}
