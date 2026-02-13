export type Photo = {
  id: number;
  url: string;
  publicId?: string;
};

export type Member = {
  id: string;
  displayName: string;
  imageUrl?: string;
  dateOfBirth: string;
  created: string;
  lastActive: string;
  gender: string;
  description?: string;
  city: string;
  country: string;
  photos: Photo[];
};
