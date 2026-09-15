import { useMemo } from "react";
import {
  flexRender,
  getCoreRowModel,
  useReactTable,
  type ColumnDef,
} from "@tanstack/react-table";
import type { SupportRequest } from "@/types/supportRequest.types";
import { Badge } from "@/components/Badge";
import {
  RequestActions,
  type RequestActionsProps,
} from "@/components/RequestActions";
import { formatDate } from "@/utils/dateFormatter";
export function SupportRequestsTable({
  items,
  onDetails,
  onDelete,
}: { items: SupportRequest[] } & Omit<RequestActionsProps, "request">) {
  const columns = useMemo<ColumnDef<SupportRequest>[]>(
    () => [
      {
        accessorKey: "title",
        header: "Título",
        cell: ({ row }) => (
          <span
            title={row.original.title}
            className="block max-w-[210px] truncate font-medium text-slate-900"
          >
            {row.original.title}
          </span>
        ),
      },
      {
        accessorKey: "requester",
        header: "Solicitante",
        cell: ({ row }) => (
          <span
            className="block max-w-[140px] truncate"
            title={row.original.requester}
          >
            {row.original.requester}
          </span>
        ),
      },
      {
        accessorKey: "priority",
        header: "Prioridade",
        cell: ({ row }) => <Badge value={row.original.priority} />,
      },
      {
        accessorKey: "status",
        header: "Status",
        cell: ({ row }) => <Badge value={row.original.status} />,
      },
      {
        accessorKey: "createdAt",
        header: "Data de criação",
        cell: ({ row }) => formatDate(row.original.createdAt),
      },
      {
        accessorKey: "completedAt",
        header: "Data de conclusão",
        cell: ({ row }) => formatDate(row.original.completedAt),
      },
      {
        id: "actions",
        header: "Ações",
        cell: ({ row }) => (
          <RequestActions
            request={row.original}
            onDetails={onDetails}
            onDelete={onDelete}
          />
        ),
      },
    ],
    [onDetails, onDelete],
  );
  const table = useReactTable({
    data: items,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });
  return (
    <div className="hidden overflow-x-auto md:block">
      <table className="w-full text-left text-xs">
        <thead className="border-y border-slate-200 bg-slate-50 text-slate-500">
          {table.getHeaderGroups().map((group) => (
            <tr key={group.id}>
              {group.headers.map((header) => (
                <th
                  scope="col"
                  key={header.id}
                  className="whitespace-nowrap px-4 py-3 font-medium"
                >
                  {flexRender(
                    header.column.columnDef.header,
                    header.getContext(),
                  )}
                </th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody className="divide-y divide-slate-100">
          {table.getRowModel().rows.map((row) => (
            <tr key={row.id} className="hover:bg-slate-50/70">
              {row.getVisibleCells().map((cell) => (
                <td key={cell.id} className="px-4 py-4">
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
