import { it, expect, vi } from "vitest";
import { AxiosHeaders } from "axios";
it("adiciona Bearer somente às chamadas protegidas", async () => {
  vi.stubEnv("NEXT_PUBLIC_API_JWT_TOKEN", "test-token");
  vi.resetModules();
  const { apiClient } = await import("@/services/apiClient");
  const headers: string[] = [];
  apiClient.defaults.adapter = async (config) => {
    headers.push(String(config.headers.Authorization ?? ""));
    return {
      data: {},
      status: 200,
      statusText: "OK",
      headers: new AxiosHeaders(),
      config,
    };
  };
  try {
    await apiClient.get("/support-requests");
    await apiClient.get("/health");
    expect(headers).toEqual(["Bearer test-token", ""]);
  } finally {
    vi.unstubAllEnvs();
  }
});
