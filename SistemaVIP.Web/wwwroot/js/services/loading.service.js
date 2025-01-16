// loading.service.js
class LoadingService {
    constructor() {
        this.overlay = null;
        this.spinner = null;
        this.timeElement = null;
        this.elapsedTime = 0;
        this.intervalId = null;
        this.isLoading = false;
        this.init();
    }

    init() {
        // Crear el overlay
        this.overlay = document.createElement('div');
        this.overlay.className = 'loading-overlay d-none';
        this.overlay.innerHTML = `
            <div class="loading-spinner">
                <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2 text-primary">Cargando... <span class="elapsed-time">0</span>s</p>
            </div>
        `;

        // Agregar estilos
        const styles = document.createElement('style');
        styles.textContent = `
            .loading-overlay {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.7);
                display: flex;
                justify-content: center;
                align-items: center;
                z-index: 9999;
            }

            .loading-spinner {
                background-color: white;
                padding: 2rem 3rem;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                text-align: center;
            }

            .d-none {
                display: none !important;
            }
        `;

        document.head.appendChild(styles);
        document.body.appendChild(this.overlay);
        this.timeElement = this.overlay.querySelector('.elapsed-time');
    }

    show() {
        if (!this.isLoading) {
            this.isLoading = true;
            this.overlay.classList.remove('d-none');
            this.elapsedTime = 0;
            this.startTimer();
        }
    }

    hide() {
        if (this.isLoading) {
            this.isLoading = false;
            this.overlay.classList.add('d-none');
            this.stopTimer();
            this.elapsedTime = 0;
            if (this.timeElement) {
                this.timeElement.textContent = '0';
            }
        }
    }

    startTimer() {
        this.intervalId = setInterval(() => {
            this.elapsedTime++;
            if (this.timeElement) {
                this.timeElement.textContent = this.elapsedTime;
            }
        }, 1000);
    }

    stopTimer() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
    }
}

// Exportar como singleton
window.loadingService = new LoadingService();