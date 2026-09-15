import { Search, SlidersHorizontal } from "lucide-react";
import { SelectField } from "@/components/Select";
import { priorityLabels, statusLabels } from "@/utils/supportRequestLabels";
import type { SupportRequestListQuery } from "@/types/supportRequest.types";
export function SupportRequestFilters({
  query,
  onChange,
}: {
  query: SupportRequestListQuery;
  onChange: (query: SupportRequestListQuery) => void;
}) {
  const active = Boolean(query.search || query.status || query.priority);
  return (
    <section className="card mb-6 p-5" aria-label="Filtros">
      <div className="mb-4 flex items-center justify-between">
        <h2 className="flex items-center gap-2 text-sm font-semibold">
          <SlidersHorizontal size={16} className="text-indigo-500" />
          Filtrar solicitações
        </h2>
        {active && (
          <button
            onClick={() => onChange({ page: 1, limit: query.limit })}
            className="text-xs font-medium text-indigo-700 hover:underline"
          >
            Limpar filtros
          </button>
        )}
      </div>
      <div className="grid gap-3 md:grid-cols-[2fr_1fr_1fr]">
        <div className="relative">
          <Search size={18} className="absolute left-3 top-3 text-slate-400" />
          <input
            aria-label="Pesquisar por título ou solicitante"
            className="field pl-10"
            placeholder="Pesquisar por título ou solicitante..."
            value={query.search ?? ""}
            onChange={(e) =>
              onChange({ ...query, search: e.target.value, page: 1 })
            }
          />
        </div>
        <SelectField
          label="Status"
          value={query.status ?? "all"}
          onChange={(value) =>
            onChange({
              ...query,
              status:
                value === "all"
                  ? undefined
                  : (value as SupportRequestListQuery["status"]),
              page: 1,
            })
          }
          options={{ all: "Todos os status", ...statusLabels }}
        />
        <SelectField
          label="Prioridade"
          value={query.priority ?? "all"}
          onChange={(value) =>
            onChange({
              ...query,
              priority:
                value === "all"
                  ? undefined
                  : (value as SupportRequestListQuery["priority"]),
              page: 1,
            })
          }
          options={{ all: "Todas as prioridades", ...priorityLabels }}
        />
      </div>
    </section>
  );
}
