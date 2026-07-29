'use client';

import { getCuentasBancarias, type CuentaBancariaTesoreria } from '@/features/tesoreria/services/tesoreriaService';
import { useQuery } from '@tanstack/react-query';

export function useGetCuentasBancarias() {
    return useQuery<CuentaBancariaTesoreria[]>({
        queryKey: ['tesoreria', 'cuentasBancarias'],
        queryFn: getCuentasBancarias,
    });
}
