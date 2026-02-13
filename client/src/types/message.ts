export type Message = {
  id: number;
  senderId: string;
  senderName: string;
  senderPhotoUrl?: string;
  recipientId: string;
  recipientName: string;
  recipientPhotoUrl?: string;
  content: string;
  dateSent: string;
  dateRead?: string;
};

export type CreateMessage = {
  recipientId: string;
  content: string;
};
