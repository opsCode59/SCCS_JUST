export class Group {
  name: string | undefined;
  connections: Connection[] | undefined;
}

export interface Connection {
  connectionId: string;
  username: string;
}
