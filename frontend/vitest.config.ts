import { defineConfig } from "vitest/config";
import { fileURLToPath } from "node:url";
export default defineConfig({
  resolve: { alias: { "@": fileURLToPath(new URL("./src", import.meta.url)) } },
  test: {
    environment: "jsdom",
    setupFiles: ["./tests/setup.ts"],
    include: ["tests/unit/**/*.spec.ts", "tests/components/**/*.spec.tsx"],
    coverage: {
      provider: "v8",
      include: [
        "src/schemas/**",
        "src/utils/**",
        "src/services/**",
        "src/hooks/**",
        "src/components/**",
      ],
    },
  },
});
