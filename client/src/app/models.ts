export interface Device {
  id: number;
  name: string;
  manufacturer: string;
  type: 'phone' | 'tablet';
  os: string;
  osVersion: string;
  processor: string;
  ramGB: number;
  description: string;
  userId: number;
  userName: string;
  userRole: string;
  userLocation: string;
}

export interface User {
  id: number;
  name: string;
  role: string;
  location: string;
  email?: string;
  password?: string;
  token?: string;
}

