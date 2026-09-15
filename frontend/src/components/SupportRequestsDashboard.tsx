import { useState } from "react";
import {
  PlusCircle,
  ChevronLeft,
  ChevronRight,
  ListFilter,
} from "lucide-react";
import { useSupportRequests } from "@/hooks/useSupportRequests";
import { useDebouncedValue } from "@/hooks/useDebouncedValue";
import { SupportRequestFilters } from "./filters/SupportRequestFilters";
import { SupportRequestsTable } from "./tables/SupportRequestsTable";
import { SupportRequestMobileCard } from "./cards/SupportRequestMobileCard";
import { LoadingState } from "./states/LoadingState";
import { EmptyState } from "./states/EmptyState";
import { ErrorState } from "./states/ErrorState";
import { SupportRequestFormModal } from "./modals/SupportRequestFormModal";
import { SupportRequestDetailsModal } from "./modals/SupportRequestDetailsModal";
import { DeleteSupportRequestDialog } from "./dialogs/DeleteSupportRequestDialog";
import { SelectField } from "./Select";
import type {
  SupportRequest,
  SupportRequestListQuery,
} from "@/types/supportRequest.types";
export function SupportRequestsDashboard() {
  const [filters, setFilters] = useState<SupportRequestListQuery>({
    page: 1,
    limit: 10,
  });
  const debouncedSearch = useDebouncedValue(filters.search?.trim() ?? "");
  const [create, setCreate] = useState(false);
  const [detail, setDetail] = useState<SupportRequest | null>(null);
  const [deletion, setDeletion] = useState<SupportRequest | null>(null);
  const query = useSupportRequests({
    ...filters,
    search: debouncedSearch || undefined,
  });
  const data = query.data;
  const filtered = Boolean(
    filters.status || filters.priority || filters.search,
  );
  return (
    <>
      <div className="mb-8 flex flex-col justify-between gap-5 sm:flex-row sm:items-center">
        <div>
          <p className="mb-2 text-xs font-semibold uppercase tracking-[0.18em] text-indigo-600">
            Central de atendimento
          </p>
          <h1 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">
            Solicitações de Suporte
          </h1>
          <p className="mt-2 text-sm leading-6 text-slate-500">
            Gerencie e acompanhe as solicitações internas da organização.
          </p>
        </div>
        <button
          className="btn-primary shrink-0"
          onClick={() => setCreate(true)}
        >
          <PlusCircle size={18} />
          Nova Solicitação
        </button>
      </div>
      <SupportRequestFilters query={filters} onChange={setFilters} />
      <section className="card overflow-hidden" aria-label="Solicitações">
        <div className="flex items-center justify-between gap-3 p-5">
          <h2 className="flex items-center gap-2 text-sm font-semibold">
            <ListFilter size={18} className="text-indigo-500" />
            Solicitações{" "}
            <span className="rounded-md bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
              {data?.totalItems ?? "—"}
            </span>
          </h2>
          <span className="text-xs text-slate-500">Mais recentes primeiro</span>
        </div>
        {query.isPending ? (
          <LoadingState />
        ) : query.isError ? (
          <ErrorState error={query.error} retry={() => void query.refetch()} />
        ) : (
          data && (
            <>
              {data.items.length === 0 ? (
                <EmptyState filtered={filtered} />
              ) : (
                <>
                  <SupportRequestsTable
                    items={data.items}
                    onDetails={setDetail}
                    onDelete={setDeletion}
                  />
                  {data.items.map((request) => (
                    <SupportRequestMobileCard
                      key={request.id}
                      request={request}
                      onDetails={setDetail}
                      onDelete={setDeletion}
                    />
                  ))}
                </>
              )}
              <div className="flex flex-wrap items-center justify-between gap-4 border-t border-slate-200 p-5 text-xs text-slate-500">
                <p>
                  {data.totalItems === 0
                    ? "0 solicitações"
                    : `Mostrando ${data.items.length ? (data.page - 1) * data.limit + 1 : 0}–${data.items.length ? (data.page - 1) * data.limit + data.items.length : 0} de ${data.totalItems} solicitações`}
                </p>
                <div className="flex flex-wrap items-center gap-3">
                  <SelectField
                    label="Itens por página"
                    value={String(filters.limit)}
                    options={{
                      10: "10 por página",
                      30: "30 por página",
                      50: "50 por página",
                      100: "100 por página",
                    }}
                    onChange={(value) =>
                      setFilters({ ...filters, limit: Number(value), page: 1 })
                    }
                  />
                  <button
                    className="icon-button"
                    aria-label="Anterior"
                    disabled={!data.hasPreviousPage}
                    onClick={() =>
                      setFilters({
                        ...filters,
                        page: Math.max(1, data.page - 1),
                      })
                    }
                  >
                    <ChevronLeft size={18} />
                  </button>
                  <span>
                    {data.page} / {Math.max(1, data.totalPages)}
                  </span>
                  <button
                    className="icon-button"
                    aria-label="Próxima"
                    disabled={!data.hasNextPage}
                    onClick={() =>
                      setFilters({ ...filters, page: data.page + 1 })
                    }
                  >
                    <ChevronRight size={18} />
                  </button>
                </div>
              </div>
            </>
          )
        )}
      </section>
      {create && <SupportRequestFormModal onClose={() => setCreate(false)} />}{" "}
      {detail && (
        <SupportRequestDetailsModal
          id={detail.id}
          onClose={() => setDetail(null)}
        />
      )}{" "}
      {deletion && (
        <DeleteSupportRequestDialog
          request={deletion}
          onClose={() => setDeletion(null)}
          onDeleted={() => {
            if (detail?.id === deletion.id) setDetail(null);
            setDeletion(null);
            if (data?.items.length === 1 && filters.page > 1)
              setFilters({ ...filters, page: filters.page - 1 });
          }}
        />
      )}
    </>
  );
}
