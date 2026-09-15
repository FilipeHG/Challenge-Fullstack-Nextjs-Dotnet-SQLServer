import next from "eslint-config-next/core-web-vitals";
import ts from "eslint-config-next/typescript";
const config = [
  ...next,
  ...ts,
  {
    ignores: [
      ".next/**",
      ".npm-cache/**",
      "coverage/**",
      "playwright-report/**",
      "test-results/**",
      "next-env.d.ts",
    ],
  },
  {
    files: ["src/components/tables/SupportRequestsTable.tsx"],
    rules: { "react-hooks/incompatible-library": "off" },
  },
];
// React Compiler is not enabled; TanStack Table v8 is intentionally used without compiler memoization.
export default config;
