/// <reference types="vite/client" />

declare global {
  interface Window {
    ENV: {
      SERVER_API_URL: string;
      CLIENT_API_URL: string;
    };
  }
}

export {};
