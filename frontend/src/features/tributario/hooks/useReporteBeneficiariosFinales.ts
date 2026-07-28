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
                tipoDocumento: String(item?.tipoDocumento ?? ''),
                numeroDocumento: String(item?.numeroDocumento ?? ''),
                nombres: String(item?.nombres ?? ''),
                apellidos: String(item?.apellidos ?? ''),
                paisResidencia: String(item?.paisResidencia ?? ''),
                cargoORol: String(item?.cargoORol ?? ''),
            }));
        },
        enabled,
    });
}
