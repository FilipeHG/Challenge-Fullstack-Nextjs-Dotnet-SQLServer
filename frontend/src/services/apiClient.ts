import axios from "axios";
export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_BASE_URL,
  timeout: 15000,
  headers: { Accept: "application/json" },
});
apiClient.interceptors.request.use((config) => {
  const token = process.env.NEXT_PUBLIC_API_JWT_TOKEN;
  if (token && config.url !== "/health")
    config.headers.Authorization = `Bearer ${token}`;
  return config;
});
