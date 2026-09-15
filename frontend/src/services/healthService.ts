import { apiClient } from "./apiClient";
export async function getApiHealth() {
  return (await apiClient.get<{ status: string }>("/health")).data;
}
