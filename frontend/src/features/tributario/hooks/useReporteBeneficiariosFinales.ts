'use client';

import { useQuery } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type BeneficiarioFinalItem = {
    tipoDocumento: string;
    numeroDocumento: string;
    nombres: string;
    apellidos: string;
    paisResidencia: string;
    cargoORol: string;
};

type UseReporteBeneficiariosFinalesParams = {
    enabled?: boolean;
};

export function useReporteBeneficiariosFinales({ enabled = true }: UseReporteBeneficiariosFinalesParams = {}) {
    return useQuery<BeneficiarioFinalItem[]>({
        queryKey: ['tributario', 'beneficiarios-finales'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/tributario/beneficiarios-finales');

            return (response.data ?? []).map((item) => ({
                tipoDocumento: String(item?.tipoDocumento ?? item?.TipoDocumento ?? ''),
                numeroDocumento: String(item?.numeroDocumento ?? item?.NumeroDocumento ?? ''),
                nombres: String(item?.nombres ?? item?.Nombres ?? ''),
                apellidos: String(item?.apellidos ?? item?.Apellidos ?? ''),
                paisResidencia: String(item?.paisResidencia ?? item?.PaisResidencia ?? ''),
                cargoORol: String(item?.cargoORol ?? item?.CargoORol ?? ''),
            }));
        },
        enabled,
    });
}
