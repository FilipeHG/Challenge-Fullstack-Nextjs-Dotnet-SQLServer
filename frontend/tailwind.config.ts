import type { Config } from "tailwindcss";
import forms from "@tailwindcss/forms";
export default {
  content: ["./src/**/*.{ts,tsx}"],
  theme: { extend: {} },
  plugins: [forms],
} satisfies Config;
