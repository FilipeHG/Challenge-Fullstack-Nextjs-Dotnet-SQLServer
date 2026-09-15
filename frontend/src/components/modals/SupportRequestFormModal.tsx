import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { Modal } from "@/components/Modal";
import { SelectField } from "@/components/Select";
import { supportRequestSchema } from "@/schemas/supportRequest.schema";
import { priorityLabels } from "@/utils/supportRequestLabels";
import { useSupportRequestMutations } from "@/hooks/useSupportRequestMutations";
import type { CreateSupportRequestPayload } from "@/types/supportRequest.types";
export function SupportRequestFormModal({ onClose }: { onClose: () => void }) {
  const { create } = useSupportRequestMutations();
  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<CreateSupportRequestPayload>({
    resolver: zodResolver(supportRequestSchema),
    defaultValues: {
      title: "",
      requester: "",
      description: "",
      priority: "medium",
    },
  });
  return (
    <Modal
      title="Nova Solicitação"
      description="Preencha os dados para abrir uma solicitação de suporte."
      onClose={onClose}
      busy={create.isPending}
    >
      <form
        noValidate
        onSubmit={handleSubmit((data) => {
          if (!create.isPending) create.mutate(data, { onSuccess: onClose });
        })}
        className="space-y-4"
      >
        {(["title", "requester"] as const).map((name) => (
          <div key={name}>
            <label className="label" htmlFor={name}>
              {name === "title" ? "Título" : "Solicitante"}{" "}
              <span className="text-rose-500">*</span>
            </label>
            <input
              id={name}
              className="field"
              {...register(name)}
              maxLength={200}
              aria-invalid={Boolean(errors[name])}
              aria-describedby={`${name}-error`}
              disabled={create.isPending}
            />
            <p id={`${name}-error`} className="field-error">
              {errors[name]?.message}
            </p>
          </div>
        ))}
        <div>
          <label className="label" htmlFor="create-priority">
            Prioridade
          </label>
          <Controller
            name="priority"
            control={control}
            render={({ field }) => (
              <SelectField
                id="create-priority"
                label="Prioridade da solicitação"
                value={field.value}
                onChange={field.onChange}
                options={priorityLabels}
                disabled={create.isPending}
              />
            )}
          />
        </div>
        <div>
          <label className="label" htmlFor="description">
            Descrição <span className="text-rose-500">*</span>
          </label>
          <textarea
            id="description"
            rows={5}
            maxLength={4000}
            className="field resize-y"
            {...register("description")}
            aria-invalid={Boolean(errors.description)}
            aria-describedby="description-error description-help"
            disabled={create.isPending}
          />
          <p id="description-help" className="mt-1 text-xs text-slate-500">
            Descreva o problema. Máximo de 4.000 caracteres.
          </p>
          <p id="description-error" className="field-error">
            {errors.description?.message}
          </p>
        </div>
        <div className="flex justify-end gap-3 border-t pt-5">
          <button
            type="button"
            className="btn-secondary"
            onClick={onClose}
            disabled={create.isPending}
          >
            Cancelar
          </button>
          <button
            type="submit"
            className="btn-primary"
            disabled={create.isPending}
          >
            {create.isPending && <Loader2 size={16} className="animate-spin" />}
            {create.isPending ? "Criando..." : "Criar Solicitação"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
