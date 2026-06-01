import { subirImagenProducto } from '@/features/merchandising/services/merchandisingService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

type SubirImagenProductoParams = {
    productoId: string;
    file: File;
};

export function useSubirImagenProducto() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ productoId, file }: SubirImagenProductoParams) => subirImagenProducto(productoId, file),
        onSuccess: async (_, variables) => {
            await queryClient.invalidateQueries({ queryKey: ['merchandising', 'productos'] });
            await queryClient.invalidateQueries({ queryKey: ['merchandising', 'productos', variables.productoId, 'movimientos'] });
        },
    });
}
