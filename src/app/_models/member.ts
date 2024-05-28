import { Photo } from "./Photo"

export interface member {
  userName: string
  gender?: string
  age: number
  photoUrl:string
  dateOfBirth: string
  knownAs: string
  created: string
  lastActive: string
  introduction?: string
  lookingFor?: string
  interests?: string
  city: string
  country: string
  photos: Photo[]
}

