import * as Select from "@radix-ui/react-select";
import { Check, ChevronDown } from "lucide-react";
export function SelectField({
  label,
  value,
  onChange,
  options,
  disabled,
  id,
  invalid,
  describedBy,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  options: Record<string, string>;
  disabled?: boolean;
  id?: string;
  invalid?: boolean;
  describedBy?: string;
}) {
  return (
    <Select.Root value={value} onValueChange={onChange} disabled={disabled}>
      <Select.Trigger
        id={id}
        className="field flex items-center justify-between gap-3"
        aria-label={label}
        aria-invalid={invalid}
        aria-describedby={describedBy}
      >
        <Select.Value placeholder={label} />
        <Select.Icon>
          <ChevronDown size={16} />
        </Select.Icon>
      </Select.Trigger>
      <Select.Portal>
        <Select.Content
          position="popper"
          sideOffset={5}
          className="z-[80] max-h-72 min-w-[var(--radix-select-trigger-width)] overflow-auto rounded-xl border border-slate-200 bg-white p-1 shadow-xl"
        >
          <Select.Viewport>
            {Object.entries(options).map(([key, text]) => (
              <Select.Item
                key={key}
                value={key}
                className="relative cursor-pointer rounded-lg py-2 pl-8 pr-4 text-sm outline-none data-[highlighted]:bg-indigo-50 data-[highlighted]:text-indigo-700"
              >
                <Select.ItemIndicator className="absolute left-2 top-2.5">
                  <Check size={14} />
                </Select.ItemIndicator>
                <Select.ItemText>{text}</Select.ItemText>
              </Select.Item>
            ))}
          </Select.Viewport>
        </Select.Content>
      </Select.Portal>
    </Select.Root>
  );
}
