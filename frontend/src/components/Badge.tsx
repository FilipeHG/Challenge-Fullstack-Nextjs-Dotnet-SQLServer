import { priorityLabels, statusLabels } from "@/utils/supportRequestLabels";
import type {
  SupportRequestPriority,
  SupportRequestStatus,
} from "@/types/supportRequest.types";
const colors = {
  low: "bg-emerald-50 text-emerald-800 ring-emerald-200",
  medium: "bg-amber-50 text-amber-800 ring-amber-200",
  high: "bg-rose-50 text-rose-800 ring-rose-200",
  open: "bg-blue-50 text-blue-800 ring-blue-200",
  inProgress: "bg-indigo-50 text-indigo-800 ring-indigo-200",
  completed: "bg-emerald-50 text-emerald-800 ring-emerald-200",
};
export function Badge({
  value,
}: {
  value: SupportRequestPriority | SupportRequestStatus;
}) {
  return (
    <span
      className={`inline-flex items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-1 text-xs font-medium ring-1 ring-inset ${colors[value]}`}
    >
      <span className="h-1.5 w-1.5 rounded-full bg-current" />
      {value in priorityLabels
        ? priorityLabels[value as SupportRequestPriority]
        : statusLabels[value as SupportRequestStatus]}
    </span>
  );
}
