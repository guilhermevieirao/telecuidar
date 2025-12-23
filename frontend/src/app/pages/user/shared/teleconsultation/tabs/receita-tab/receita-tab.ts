import { Component, Input, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { ButtonComponent } from '@shared/components/atoms/button/button';
import { IconComponent } from '@shared/components/atoms/icon/icon';
import { 
  PrescriptionService, 
  Prescription, 
  PrescriptionItem, 
  AddPrescriptionItemDto,
  MedicamentoAnvisa,
  DigitalCertificate
} from '@core/services/prescription.service';
import { CertificateService, InstalledCertificate } from '@core/services/certificate.service';
import { ModalService } from '@core/services/modal.service';

@Component({
  selector: 'app-receita-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, ButtonComponent, IconComponent],
  templateUrl: './receita-tab.html',
  styleUrls: ['./receita-tab.scss']
})
export class ReceitaTabComponent implements OnInit, OnDestroy {
  @Input() appointmentId: string | null = null;
  @Input() userrole: 'PATIENT' | 'PROFESSIONAL' | 'ADMIN' = 'PATIENT';
  @Input() readonly = false;
  
  @ViewChild('pfxFileInput') pfxFileInput!: ElementRef<HTMLInputElement>;

  prescription: Prescription | null = null;
  isLoading = true;
  isSaving = false;
  isGeneratingPdf = false;
  showItemForm = false;
  
  // Form para novo item
  itemForm: FormGroup;
  
  // Busca de medicamentos
  medicamentoSearch = '';
  medicamentoResults: MedicamentoAnvisa[] = [];
  showMedicamentoDropdown = false;
  isSearching = false;
  
  // Certificados digitais instalados
  installedCertificates: InstalledCertificate[] = [];
  selectedInstalledCert: InstalledCertificate | null = null;
  showInstalledCertsModal = false;
  isLoadingInstalledCerts = false;
  webPkiAvailable = false;
  
  // Certificados digitais (legacy - PFX file)
  certificates: DigitalCertificate[] = [];
  showCertificateModal = false;
  selectedCertificate: DigitalCertificate | null = null;
  isLoadingCertificates = false;
  isSigning = false;
  
  // PFX file upload (fallback)
  pfxFile: File | null = null;
  pfxPassword = '';
  showPfxPasswordModal = false;

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  constructor(
    private fb: FormBuilder,
    private prescriptionService: PrescriptionService,
    private certificateService: CertificateService,
    private modalService: ModalService
  ) {
    this.itemForm = this.fb.group({
      medicamento: ['', Validators.required],
      codigoAnvisa: [''],
      dosagem: ['', Validators.required],
      frequencia: ['', Validators.required],
      periodo: ['', Validators.required],
      posologia: ['', Validators.required],
      observacoes: ['']
    });
  }

  ngOnInit() {
    if (this.appointmentId) {
      this.loadPrescription();
    }

    // Verificar se Web PKI está disponível
    this.checkWebPkiAvailability();

    // Setup debounced search
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      if (query.length >= 2) {
        this.searchMedicamentos(query);
      } else {
        this.medicamentoResults = [];
        this.showMedicamentoDropdown = false;
      }
    });
  }

  async checkWebPkiAvailability() {
    this.webPkiAvailable = await this.certificateService.isAvailable();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadPrescription() {
    if (!this.appointmentId) return;

    this.isLoading = true;
    this.prescriptionService.getPrescriptionByAppointment(this.appointmentId).subscribe({
      next: (prescription) => {
        this.prescription = prescription;
        this.isLoading = false;
      },
      error: (error) => {
        if (error.status === 404) {
          // Receita não existe ainda, vamos criar
          this.prescription = null;
        }
        this.isLoading = false;
      }
    });
  }

  createPrescription() {
    if (!this.appointmentId) return;

    this.isSaving = true;
    this.prescriptionService.createPrescription({ appointmentId: this.appointmentId }).subscribe({
      next: (prescription) => {
        this.prescription = prescription;
        this.isSaving = false;
        this.showItemForm = true;
      },
      error: (error) => {
        this.isSaving = false;
        this.modalService.alert({
          title: 'Erro',
          message: error.error?.message || 'Erro ao criar receita.',
          variant: 'danger'
        }).subscribe();
      }
    });
  }

  onMedicamentoInput(event: Event) {
    const input = event.target as HTMLInputElement;
    this.medicamentoSearch = input.value;
    this.searchSubject.next(input.value);
  }

  searchMedicamentos(query: string) {
    this.isSearching = true;
    this.prescriptionService.searchMedicamentos(query).subscribe({
      next: (results) => {
        this.medicamentoResults = results;
        this.showMedicamentoDropdown = results.length > 0;
        this.isSearching = false;
      },
      error: () => {
        this.isSearching = false;
        this.medicamentoResults = [];
      }
    });
  }

  selectMedicamento(medicamento: MedicamentoAnvisa) {
    this.itemForm.patchValue({
      medicamento: medicamento.nome,
      codigoAnvisa: medicamento.codigo
    });
    this.medicamentoSearch = medicamento.nome;
    this.showMedicamentoDropdown = false;
  }

  useFreeText() {
    this.itemForm.patchValue({
      medicamento: this.medicamentoSearch,
      codigoAnvisa: ''
    });
    this.showMedicamentoDropdown = false;
  }

  hideDropdown() {
    setTimeout(() => {
      this.showMedicamentoDropdown = false;
    }, 200);
  }

  addItem() {
    if (this.itemForm.invalid || !this.prescription) return;

    const item: AddPrescriptionItemDto = this.itemForm.value;
    
    this.isSaving = true;
    this.prescriptionService.addItem(this.prescription.id, item).subscribe({
      next: (prescription) => {
        this.prescription = prescription;
        this.itemForm.reset();
        this.medicamentoSearch = '';
        this.isSaving = false;
        this.showItemForm = false;
      },
      error: (error) => {
        this.isSaving = false;
        this.modalService.alert({
          title: 'Erro',
          message: error.error?.message || 'Erro ao adicionar medicamento.',
          variant: 'danger'
        }).subscribe();
      }
    });
  }

  removeItem(itemId: string) {
    if (!this.prescription) return;

    this.modalService.confirm({
      title: 'Remover Medicamento',
      message: 'Tem certeza que deseja remover este medicamento da receita?',
      variant: 'warning',
      confirmText: 'Sim, remover',
      cancelText: 'Cancelar'
    }).subscribe(result => {
      if (result.confirmed && this.prescription) {
        this.prescriptionService.removeItem(this.prescription.id, itemId).subscribe({
          next: (prescription) => {
            this.prescription = prescription;
          },
          error: (error) => {
            this.modalService.alert({
              title: 'Erro',
              message: error.error?.message || 'Erro ao remover medicamento.',
              variant: 'danger'
            }).subscribe();
          }
        });
      }
    });
  }

  cancelItemForm() {
    this.showItemForm = false;
    this.itemForm.reset();
    this.medicamentoSearch = '';
  }

  deletePrescription() {
    if (!this.prescription) return;

    this.modalService.confirm({
      title: 'Excluir Receita',
      message: 'Tem certeza que deseja excluir esta receita? Esta ação não pode ser desfeita.',
      variant: 'danger',
      confirmText: 'Sim, excluir',
      cancelText: 'Cancelar'
    }).subscribe(result => {
      if (result.confirmed && this.prescription) {
        this.isSaving = true;
        this.prescriptionService.deletePrescription(this.prescription.id).subscribe({
          next: () => {
            this.prescription = null;
            this.isSaving = false;
            this.modalService.alert({
              title: 'Sucesso',
              message: 'Receita excluída com sucesso.',
              variant: 'success'
            }).subscribe();
          },
          error: (error) => {
            this.isSaving = false;
            this.modalService.alert({
              title: 'Erro',
              message: error.error?.message || 'Erro ao excluir receita.',
              variant: 'danger'
            }).subscribe();
          }
        });
      }
    });
  }

  // === Assinatura Digital ===
  
  async loadCertificates() {
    this.isLoadingCertificates = true;
    
    // Simular carregamento de certificados ICP-Brasil do computador
    // Em produção, usar Web Crypto API ou biblioteca como LacunaWebPKI
    try {
      // Simulação - em produção, usar:
      // - LacunaWebPKI (https://www.lacunasoftware.com/pt/web-pki)
      // - Certisign
      // - ou outra biblioteca compatível com ICP-Brasil
      
      this.certificates = await this.detectCertificates();
      this.isLoadingCertificates = false;
      
      if (this.certificates.length === 0) {
        this.modalService.alert({
          title: 'Nenhum Certificado Encontrado',
          message: 'Não foi possível encontrar certificados digitais A1 (ICP-Brasil) instalados neste computador. Verifique se o certificado está instalado corretamente.',
          variant: 'warning'
        }).subscribe();
      } else if (this.certificates.length === 1) {
        this.selectedCertificate = this.certificates[0];
        this.signDocument();
      } else {
        this.showCertificateModal = true;
      }
    } catch (error) {
      this.isLoadingCertificates = false;
      this.modalService.alert({
        title: 'Erro',
        message: 'Erro ao carregar certificados digitais. Verifique se há um certificado A1 válido instalado.',
        variant: 'danger'
      }).subscribe();
    }
  }

  private async detectCertificates(): Promise<DigitalCertificate[]> {
    // Em produção, usar Web Crypto API ou biblioteca específica
    // Esta é uma simulação para demonstração
    
    return new Promise((resolve) => {
      setTimeout(() => {
        // Simular detecção de certificados
        // Em ambiente real, isso listaria os certificados do Windows Certificate Store
        const mockCertificates: DigitalCertificate[] = [
          {
            thumbprint: 'A1B2C3D4E5F6789012345678901234567890ABCD',
            subject: 'CN=DR. EXEMPLO MÉDICO:12345678900, OU=AR TELECUIDAR, O=ICP-Brasil',
            issuer: 'CN=AC TELECUIDAR RFB v5, O=ICP-Brasil',
            validFrom: new Date('2024-01-01'),
            validTo: new Date('2027-01-01'),
            isValid: true
          }
        ];
        
        resolve(mockCertificates);
      }, 500);
    });
  }

  selectCertificate(cert: DigitalCertificate) {
    this.selectedCertificate = cert;
  }

  confirmCertificateSelection() {
    if (this.selectedCertificate) {
      this.showCertificateModal = false;
      this.signDocument();
    }
  }

  closeCertificateModal() {
    this.showCertificateModal = false;
    this.selectedCertificate = null;
  }

  async signDocument() {
    if (!this.prescription || !this.selectedCertificate) return;

    this.isSigning = true;

    try {
      // Gerar assinatura digital
      // Em produção, usar Web Crypto API com o certificado selecionado
      const signature = await this.generateSignature();
      
      this.prescriptionService.signPrescription(this.prescription.id, {
        certificateThumbprint: this.selectedCertificate.thumbprint,
        signature: signature,
        certificateSubject: this.selectedCertificate.subject
      }).subscribe({
        next: (prescription) => {
          this.prescription = prescription;
          this.isSigning = false;
          this.modalService.alert({
            title: 'Sucesso',
            message: 'Receita assinada digitalmente com sucesso!',
            variant: 'success'
          }).subscribe();
        },
        error: (error) => {
          this.isSigning = false;
          this.modalService.alert({
            title: 'Erro',
            message: error.error?.message || 'Erro ao assinar documento.',
            variant: 'danger'
          }).subscribe();
        }
      });
    } catch (error) {
      this.isSigning = false;
      this.modalService.alert({
        title: 'Erro',
        message: 'Erro ao gerar assinatura digital.',
        variant: 'danger'
      }).subscribe();
    }
  }

  private async generateSignature(): Promise<string> {
    // Em produção, usar Web Crypto API para gerar assinatura real
    // Exemplo com LacunaWebPKI:
    // return await pki.signHash({
    //   thumbprint: this.selectedCertificate.thumbprint,
    //   hash: documentHash,
    //   digestAlgorithm: 'SHA-256'
    // });
    
    return btoa(`SIGNATURE_${Date.now()}_${this.selectedCertificate?.thumbprint}`);
  }

  // === Geração de PDF ===

  generatePdf() {
    if (!this.prescription) return;

    this.isGeneratingPdf = true;
    this.prescriptionService.generatePdf(this.prescription.id).subscribe({
      next: (pdf) => {
        this.isGeneratingPdf = false;
        this.downloadPdf(pdf.pdfBase64, pdf.fileName);
      },
      error: (error) => {
        this.isGeneratingPdf = false;
        this.modalService.alert({
          title: 'Erro',
          message: error.error?.message || 'Erro ao gerar PDF.',
          variant: 'danger'
        }).subscribe();
      }
    });
  }

  // === Assinatura com Certificados Instalados ===

  // Abrir modal para selecionar certificado instalado ou arquivo PFX
  async openSignatureOptions() {
    if (!this.prescription) return;

    this.isLoadingInstalledCerts = true;
    
    // Tentar listar certificados instalados
    const certs = await this.certificateService.listCertificates();
    
    this.isLoadingInstalledCerts = false;
    
    if (certs.length > 0) {
      // Certificados encontrados - mostrar modal de seleção
      this.installedCertificates = certs;
      this.selectedInstalledCert = null;
      this.showInstalledCertsModal = true;
    } else {
      // Nenhum certificado encontrado - oferecer opções
      this.modalService.confirm({
        title: 'Selecionar Certificado',
        message: 'Não foram encontrados certificados digitais instalados neste computador. Deseja selecionar um arquivo de certificado (.pfx)?',
        confirmText: 'Selecionar Arquivo',
        cancelText: 'Cancelar'
      }).subscribe(confirmed => {
        if (confirmed) {
          this.openPfxSelector();
        }
      });
    }
  }

  // Selecionar certificado instalado
  selectInstalledCertificate(cert: InstalledCertificate) {
    this.selectedInstalledCert = cert;
  }

  // Fechar modal de certificados instalados
  closeInstalledCertsModal() {
    this.showInstalledCertsModal = false;
    this.selectedInstalledCert = null;
    this.installedCertificates = [];
  }

  // Confirmar assinatura com certificado instalado
  async confirmInstalledCertSignature() {
    if (!this.prescription || !this.selectedInstalledCert) return;

    this.isSigning = true;
    this.showInstalledCertsModal = false;

    try {
      // Gerar hash do documento para assinatura
      const documentData = JSON.stringify({
        prescriptionId: this.prescription.id,
        items: this.prescription.items,
        timestamp: Date.now()
      });

      // Assinar com o certificado selecionado
      const signatureResult = await this.certificateService.signData(
        this.selectedInstalledCert.thumbprint,
        btoa(documentData)
      );

      if (!signatureResult) {
        throw new Error('Falha ao gerar assinatura digital');
      }

      // Enviar assinatura para o backend
      this.prescriptionService.signWithInstalledCert(this.prescription.id, {
        thumbprint: this.selectedInstalledCert.thumbprint,
        subjectName: this.selectedInstalledCert.subjectName,
        signature: signatureResult.signature,
        certificateContent: signatureResult.certificateContent
      }).subscribe({
        next: (pdf) => {
          this.isSigning = false;
          this.selectedInstalledCert = null;
          
          // Download the signed PDF
          this.downloadPdf(pdf.pdfBase64, pdf.fileName);
          
          // Reload prescription to update signed status
          this.loadPrescription();
          
          this.modalService.alert({
            title: 'Sucesso',
            message: 'PDF gerado e assinado digitalmente com sucesso!',
            variant: 'success'
          }).subscribe();
        },
        error: (error) => {
          this.isSigning = false;
          this.modalService.alert({
            title: 'Erro',
            message: error.error?.message || 'Erro ao gerar PDF assinado.',
            variant: 'danger'
          }).subscribe();
        }
      });
    } catch (error: any) {
      this.isSigning = false;
      this.modalService.alert({
        title: 'Erro',
        message: error.message || 'Erro ao assinar com o certificado selecionado.',
        variant: 'danger'
      }).subscribe();
    }
  }

  // Usar arquivo PFX ao invés de certificado instalado
  useFileCertificate() {
    this.closeInstalledCertsModal();
    this.openPfxSelector();
  }

  // Trigger file input for PFX selection (fallback)
  openPfxSelector() {
    if (this.pfxFileInput) {
      this.pfxFileInput.nativeElement.click();
    }
  }

  // Handle PFX file selection
  onPfxFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.pfxFile = input.files[0];
      this.pfxPassword = '';
      this.showPfxPasswordModal = true;
    }
    // Reset input to allow selecting the same file again
    input.value = '';
  }

  // Close PFX password modal
  closePfxPasswordModal() {
    this.showPfxPasswordModal = false;
    this.pfxFile = null;
    this.pfxPassword = '';
  }

  // Generate signed PDF with PFX certificate
  generateSignedPdf() {
    if (!this.prescription || !this.pfxFile || !this.pfxPassword) {
      this.modalService.alert({
        title: 'Erro',
        message: 'Selecione um certificado PFX e informe a senha.',
        variant: 'warning'
      }).subscribe();
      return;
    }

    this.isSigning = true;
    this.showPfxPasswordModal = false;

    // Read the PFX file as base64
    const reader = new FileReader();
    reader.onload = () => {
      const base64 = (reader.result as string).split(',')[1]; // Remove data:... prefix
      
      this.prescriptionService.generateSignedPdf(this.prescription!.id, base64, this.pfxPassword).subscribe({
        next: (pdf) => {
          this.isSigning = false;
          this.pfxFile = null;
          this.pfxPassword = '';
          
          // Download the signed PDF
          this.downloadPdf(pdf.pdfBase64, pdf.fileName);
          
          // Reload prescription to update signed status
          this.loadPrescription();
          
          this.modalService.alert({
            title: 'Sucesso',
            message: 'PDF gerado e assinado digitalmente com sucesso!',
            variant: 'success'
          }).subscribe();
        },
        error: (error) => {
          this.isSigning = false;
          this.modalService.alert({
            title: 'Erro',
            message: error.error?.message || 'Erro ao assinar PDF. Verifique a senha do certificado.',
            variant: 'danger'
          }).subscribe();
        }
      });
    };
    
    reader.onerror = () => {
      this.isSigning = false;
      this.modalService.alert({
        title: 'Erro',
        message: 'Erro ao ler o arquivo do certificado.',
        variant: 'danger'
      }).subscribe();
    };
    
    reader.readAsDataURL(this.pfxFile);
  }

  private downloadPdf(base64: string, fileName: string) {
    // Verificar se é HTML (para visualização) ou PDF real
    const isHtml = base64.startsWith('PCFET0NUWVBF') || !base64.startsWith('JVBERi');
    
    if (isHtml) {
      // Converter Base64 HTML para Blob e abrir em nova janela
      const htmlContent = atob(base64);
      const blob = new Blob([htmlContent], { type: 'text/html' });
      const url = URL.createObjectURL(blob);
      window.open(url, '_blank');
    } else {
      // PDF real - fazer download
      const byteCharacters = atob(base64);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: 'application/pdf' });
      
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = fileName;
      link.click();
    }
  }

  // === Helpers ===

  get canEdit(): boolean {
    return this.userrole === 'PROFESSIONAL' && !this.prescription?.isSigned && !this.readonly;
  }

  get canDelete(): boolean {
    return this.userrole === 'PROFESSIONAL' && !this.readonly;
  }

  get isProfessional(): boolean {
    return this.userrole === 'PROFESSIONAL';
  }

  get canSign(): boolean {
    return this.userrole === 'PROFESSIONAL' && 
           this.prescription !== null && 
           this.prescription.items.length > 0 && 
           !this.prescription.isSigned;
  }

  get hasItems(): boolean {
    return (this.prescription?.items?.length ?? 0) > 0;
  }

  get isSigned(): boolean {
    return this.prescription?.isSigned ?? false;
  }
}
