
function formatoFechaDiaMesAño(params) {
    if (!params.value) return '';
    // Convertir a Date y formatear como dd/MM/yyyy
    const fecha = new Date(params.value);
    const dia = String(fecha.getDate()).padStart(2, '0');
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const anio = fecha.getFullYear();
    return `${dia}/${mes}/${anio}`;
}


/**
 *  Aplica visibilidad y orden de columnas según el ancho
 */
function onResponsiveColumnsGridCabOC(params) {
   
    const api = params.api || (window.masterGrid.getGridApi?.() || window.masterGrid);
    const isMobile = window.innerWidth < 768; // breakpoint móvil

    if (isMobile) {
        // ➜ Mostrar solo lo esencial y ordenar
        api.applyColumnState({
            state: [
                { colId: 'purchId'},
                { colId: 'totalAmount'},
                { colId: 'purchStatusText'},

                { colId: 'acciones'},
                { colId: 'purchName'},
                { colId: 'orderAccount'},
                { colId: 'documentStateText'},
                { colId: 'currencyCode'}
               
            ],
            applyOrder: true
        });
        // ➜ Página más pequeña en móvil
        window.masterGrid.setGridOption('paginationPageSize', 10);
        // ➜ Ajustar columnas al ancho disponible
        api.sizeColumnsToFit?.();
    } else {
        // ➜ Desktop: mostrar todo y devolver orden original (si quieres)
        api.applyColumnState({
            state: [
                { colId: 'purchId'},
                { colId: 'purchName'},
                { colId: 'orderAccount'},
                { colId: 'documentStateText'},
                { colId: 'purchStatusText'},
                { colId: 'totalAmount'},
                { colId: 'currencyCode'},
                { colId: 'acciones'},
            ],
            applyOrder: true
        });
        window.masterGrid.setGridOption('paginationPageSize', 10);
        api.sizeColumnsToFit?.();
    }
}


function onResponsiveColumnsGridFactura(params,grid) {

    const api = params.api || (grid.getGridApi?.() || grid);
    const isMobile = window.innerWidth < 768; // breakpoint móvil

    if (isMobile) {
        // ➜ Mostrar solo lo esencial y ordenar
        api.applyColumnState({
            state: [
                { colId: 'numFactura' }, 
                { colId: 'importeFactura' },
                { colId: 'montoPendiente' },
                { colId: 'fechaDocumento' },
                { colId: 'tipoDocumento' },
                { colId: 'acciones' },

                { colId: 'ordenCompra' },
                { colId: 'correlativoInterno'},
                { colId: 'cuentaFacturacion'},
                { colId: 'asiento' },
                { colId: 'divisa' },
                { colId: 'impuesto' },
                { colId: 'empresa' },
                { colId: 'pedidoVentas' },
                { colId: 'registradoTravesEmpresaVinculada' },
                { colId: 'fechaVencimiento' }

            ],
            applyOrder: true
        });
        // ➜ Página más pequeña en móvil
        grid.setGridOption('paginationPageSize', 10);
        // ➜ Ajustar columnas al ancho disponible
        api.sizeColumnsToFit?.();
    } else {
        // ➜ Desktop: mostrar todo y devolver orden original (si quieres)
        api.applyColumnState({
            state: [
                { colId: 'numFactura' },
                { colId: 'fechaDocumento'},
                { colId: 'correlativoInterno'},
                { colId: 'tipoDocumento'},
                { colId: 'documentStateText'},
                { colId: 'cuentaFacturacion'},
                { colId: 'ordenCompra'},
                { colId: 'asiento' },
                { colId: 'divisa' },
                { colId: 'importeFactura' },
                { colId: 'impuesto' },
                { colId: 'montoPendiente' },
                { colId: 'empresa' },
                { colId: 'pedidoVentas' },
                { colId: 'registradoTravesEmpresaVinculada' },
                { colId: 'fechaVencimiento' },
                { colId: 'acciones' }
            ],
            applyOrder: true
        });
        grid.setGridOption('paginationPageSize', 10);
        api.sizeColumnsToFit?.();
    }
}

function montoMonedaFormateado(params, moneda) {
    const v = Number(params.value);
    if (isNaN(v)) return '';

    //const moneda = params.data.currencyCode;

    if (moneda === "CLP") {
        // Formato entero con puntos para miles
        const enteroConMiles = Math.round(v).toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        return `$ ${enteroConMiles}`;
    } else {
        // Formato con dos decimales, coma decimal y puntos para miles
        const parts = v.toFixed(2).split('.'); // ["entero", "decimales"]
        const enteroConMiles = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, '.');

        if (moneda == "USD") {
            return `$ ${enteroConMiles},${parts[1]}`;
        }

        if (moneda == "CLF") {
            return `UF ${enteroConMiles},${parts[1]}`;
        }

        return `${enteroConMiles},${parts[1]}`; // Ej: 1.234,56
    }
}


function montoMonedaFormateadoDosDecimales(params) {
    const v = Number(params.value);
    if (isNaN(v)) return '';

    const parts = v.toFixed(2).split('.'); // ["entero", "decimales"]
    const enteroConMiles = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, '.');

    return `${enteroConMiles},${parts[1]}`; // Ej: 1.234,56

    if (moneda === "CLP") {
        // Formato entero con puntos para miles
        const enteroConMiles = Math.round(v).toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        return `$ ${enteroConMiles}`;
    } else {
        // Formato con dos decimales, coma decimal y puntos para miles
        const parts = v.toFixed(2).split('.'); // ["entero", "decimales"]
        const enteroConMiles = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, '.');

        if (moneda == "USD") {
            return `$ ${enteroConMiles},${parts[1]}`;
        }

        if (moneda == "CLF") {
            return `UF ${enteroConMiles},${parts[1]}`;
        }

        return `${enteroConMiles},${parts[1]}`; // Ej: 1.234,56
    }
}
