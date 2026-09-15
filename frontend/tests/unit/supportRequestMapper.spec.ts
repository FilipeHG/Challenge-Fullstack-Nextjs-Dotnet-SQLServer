import { it, expect } from "vitest";
import {
  normalizePriority,
  normalizeStatus,
} from "@/utils/supportRequestMapper";
it.each([
  [1, "low"],
  [2, "medium"],
  [3, "high"],
  ["low", "low"],
  ["medium", "medium"],
  ["high", "high"],
] as const)("normaliza prioridade %s", (input, output) => {
  expect(normalizePriority(input)).toBe(output);
});
it.each([
  [1, "open"],
  [2, "inProgress"],
  [3, "completed"],
  ["open", "open"],
  ["inProgress", "inProgress"],
  ["completed", "completed"],
] as const)("normaliza status %s", (input, output) => {
  expect(normalizeStatus(input)).toBe(output);
});
