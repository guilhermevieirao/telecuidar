import { Injectable, signal, PLATFORM_ID, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { Observable, BehaviorSubject, tap, catchError, of, throwError } from 'rxjs';
import {
  Usuario,
  User,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  ForgotPasswordRequest,
  ForgotPasswordResponse,
  ResetPasswordRequest,
  ResetPasswordResponse,
  VerifyEmailRequest,
  VerifyEmailResponse,
  AuthState
} from '@app/core/models/auth.model';
import { AUTH_ENDPOINTS, STORAGE_KEYS } from '@app/core/constants/auth.constants';
import { environment } from '@env/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authState = new BehaviorSubject<AuthState>({
    user: null,
    accessToken: null,
    refreshToken: null,
    isAuthenticated: false,
    isLoading: false,
    error: null
  });

  public authState$ = this.authState.asObservable();
  public currentUser = signal<Usuario | null>(null);
  public isAuthenticated = signal<boolean>(false);
  private storageLoaded = false;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    // Load from storage only in browser
    if (isPlatformBrowser(this.platformId)) {
      this.loadUserFromStorage();
    }
  }

  // Login
  login(request: { email: string; password: string; rememberMe?: boolean }): Observable<LoginResponse> {
    this.setLoading(true);
    
    // Converter para o formato esperado pelo backend
    const loginRequest: LoginRequest = {
      email: request.email,
      senha: request.password,
      lembrarMe: request.rememberMe
    };
    
    return this.http.post<LoginResponse>(AUTH_ENDPOINTS.LOGIN, loginRequest).pipe(
      tap(response => {
        console.log('[AuthService] Login response recebida, salvando token...');
        this.handleAuthSuccess(response, request.rememberMe);
        this.setLoading(false);
      }),
      catchError(error => {
        this.setError(error.error?.mensagem || error.error?.message || 'Erro ao fazer login');
        this.setLoading(false);
        throw error;
      })
    );
  }

  // Register
  register(request: { name: string; lastName: string; email: string; cpf: string; phone?: string; password: string; confirmPassword: string; acceptTerms: boolean }): Observable<RegisterResponse> {
    this.setLoading(true);
    
    // Converter para o formato esperado pelo backend
    const registerRequest: RegisterRequest = {
      nome: request.name,
      sobrenome: request.lastName,
      email: request.email,
      cpf: request.cpf,
      telefone: request.phone,
      senha: request.password,
      confirmarSenha: request.confirmPassword,
      aceitarTermos: request.acceptTerms
    };
    
    return this.http.post<RegisterResponse>(AUTH_ENDPOINTS.REGISTER, registerRequest).pipe(
      tap(response => {
        this.setLoading(false);
      }),
      catchError(error => {
        this.setError(error.error?.mensagem || error.error?.message || 'Erro ao criar conta');
        this.setLoading(false);
        throw error;
      })
    );
  }

  // Logout
  logout(): void {
    this.clearStorage();
    this.authState.next({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      isLoading: false,
      error: null
    });
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  // Forgot Password
  forgotPassword(request: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
    this.setLoading(true);
    
    return this.http.post<ForgotPasswordResponse>(AUTH_ENDPOINTS.FORGOT_PASSWORD, request).pipe(
      tap(() => this.setLoading(false)),
      catchError(error => {
        this.setError(error.error?.message || 'Erro ao enviar email');
        this.setLoading(false);
        throw error;
      })
    );
  }

  // Reset Password
  resetPassword(request: ResetPasswordRequest): Observable<ResetPasswordResponse> {
    this.setLoading(true);
    
    return this.http.post<ResetPasswordResponse>(AUTH_ENDPOINTS.RESET_PASSWORD, request).pipe(
      tap(() => this.setLoading(false)),
      catchError(error => {
        this.setError(error.error?.message || 'Erro ao redefinir senha');
        this.setLoading(false);
        throw error;
      })
    );
  }

  // Verify Email
  verifyEmail(request: VerifyEmailRequest): Observable<VerifyEmailResponse> {
    this.setLoading(true);
    
    return this.http.get<VerifyEmailResponse>(`${AUTH_ENDPOINTS.VERIFY_EMAIL}?token=${request.token}`).pipe(
      tap(response => {
        if (response.usuario) {
          this.currentUser.set(response.usuario);
        }
        this.setLoading(false);
      }),
      catchError(error => {
        this.setError(error.error?.mensagem || error.error?.message || 'Erro ao verificar email');
        this.setLoading(false);
        throw error;
      })
    );
  }

  // Resend Verification Email
  resendVerificationEmail(email: string): Observable<any> {
    return this.http.post(AUTH_ENDPOINTS.RESEND_VERIFICATION, { email });
  }

  // Request Email Change
  requestEmailChange(novoEmail: string): Observable<{ mensagem: string }> {
    return this.http.post<{ mensagem: string }>(AUTH_ENDPOINTS.REQUEST_EMAIL_CHANGE, { novoEmail });
  }

  // Verify Email Change
  verifyEmailChange(token: string): Observable<{ mensagem: string; usuario: Usuario }> {
    return this.http.post<{ mensagem: string; usuario: Usuario }>(AUTH_ENDPOINTS.VERIFY_EMAIL_CHANGE, { token }).pipe(
      tap(response => {
        if (response.usuario) {
          this.currentUser.set(response.usuario);
          // Atualizar também no storage
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(response.usuario));
          }
        }
      })
    );
  }

  // Cancel Email Change
  cancelEmailChange(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(AUTH_ENDPOINTS.CANCEL_EMAIL_CHANGE, {});
  }

  // Google Login (placeholder)
  loginWithGoogle(): void {
    if (isPlatformBrowser(this.platformId)) {
      // Implementar OAuth2 com Google
      window.location.href = AUTH_ENDPOINTS.GOOGLE_LOGIN;
    }
  }

  // Refresh Token
  refreshToken(): Observable<LoginResponse> {
    let refreshToken: string | null = null;
    
    if (isPlatformBrowser(this.platformId)) {
      refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
    }
    
    return this.http.post<LoginResponse>(AUTH_ENDPOINTS.REFRESH_TOKEN, { refreshToken }).pipe(
      tap(response => {
        this.handleAuthSuccess(response, true);
      })
    );
  }

  // Get Access Token
  getAccessToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
    }
    return null;
  }

  // Private helper methods
  private handleAuthSuccess(response: LoginResponse, rememberMe?: boolean): void {
    
    // IMPORTANTE: Salvar token no storage PRIMEIRO
    if (isPlatformBrowser(this.platformId)) {
      console.log('[AuthService] Salvando token no storage...');
      if (rememberMe) {
        localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, response.accessToken);
        localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, response.refreshToken);
        localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(response.usuario));
        localStorage.setItem(STORAGE_KEYS.REMEMBER_ME, 'true');
      } else {
        sessionStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, response.accessToken);
        sessionStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, response.refreshToken);
        sessionStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(response.usuario));
      }
      console.log('[AuthService] Token salvo! Verificando:', rememberMe ? localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)?.substring(0, 20) : sessionStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)?.substring(0, 20));
      
      // Usar queueMicrotask para garantir que o storage está sincronizado
      // antes de emitir authState$ e disparar requisições HTTP
      queueMicrotask(() => {
        console.log('[AuthService] Emitindo authState$ com isAuthenticated=true');
        this.authState.next({
          user: response.usuario,
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
          isAuthenticated: true,
          isLoading: false,
          error: null
        });
        
        this.currentUser.set(response.usuario);
        this.isAuthenticated.set(true);
      });
    } else {
      // No SSR, emite imediatamente
      this.authState.next({
        user: response.usuario,
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
        isAuthenticated: true,
        isLoading: false,
        error: null
      });
      
      this.currentUser.set(response.usuario);
      this.isAuthenticated.set(true);
    }
  }

  private loadUserFromStorage(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    if (this.storageLoaded) {
      return;
    }

    this.storageLoaded = true;

    // Try localStorage first (remember me)
    let accessToken = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
    let refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
    let userStr = localStorage.getItem(STORAGE_KEYS.USER);
    
    console.log('[AuthService] loadUserFromStorage - Verificando localStorage...');
    console.log('[AuthService] localStorage TOKEN:', accessToken ? `${accessToken.substring(0, 20)}...` : 'NULL');
    
    // If not in localStorage, try sessionStorage
    if (!accessToken) {
      accessToken = sessionStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
      refreshToken = sessionStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
      userStr = sessionStorage.getItem(STORAGE_KEYS.USER);
      console.log('[AuthService] Token não encontrado em localStorage, tentando sessionStorage...');
      console.log('[AuthService] sessionStorage TOKEN:', accessToken ? `${accessToken.substring(0, 20)}...` : 'NULL');
    }
    
    if (accessToken && refreshToken && userStr) {
      try {
        const user = JSON.parse(userStr);
        console.log('[AuthService] Token restaurado do storage, emitindo authState$ imediatamente...');
        
        // IMPORTANTE: Emitir authState$ IMEDIATAMENTE com dados do cache
        // Isso garante que componentes consigam usar o token antes de fazer refetch
        this.authState.next({
          user,
          accessToken,
          refreshToken,
          isAuthenticated: true,
          isLoading: false,
          error: null
        });
        this.currentUser.set(user);
        this.isAuthenticated.set(true);
        
        console.log('[AuthService] authState$ emitido com sucesso! Iniciando refetch assíncrono...');
        
        // Fazer refetch de forma ASSÍNCRONA e opcional
        // Se falhar, a sessão já está restaurada (não quebra)
        setTimeout(() => {
          console.log('[AuthService] Iniciando refetch de dados do usuário (async)...');
          this.refetchCurrentUser().subscribe({
            next: (user) => {
              console.log('[AuthService] Dados do usuário refetch OK, usuário atualizado');
            },
            error: (err) => {
              console.warn('[AuthService] Erro ao refetch user data:', err.status, err.message);
              console.warn('[AuthService] Mas sessão permanece ativa com dados cached');
              // Sessão permanece ativa mesmo que refetch falhe
              // Não fazer nada aqui
            }
          });
        }, 100); // Pequeno delay para permitir que authState$ seja processado primeiro
      } catch (error) {
        console.error('[AuthService] Erro ao fazer parse do user storage:', error);
        this.clearStorage();
      }
    } else {
      console.log('[AuthService] Nenhum token encontrado no storage');
    }
  }

  private clearStorage(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
      localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
      localStorage.removeItem(STORAGE_KEYS.USER);
      localStorage.removeItem(STORAGE_KEYS.REMEMBER_ME);
      
      sessionStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
      sessionStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
      sessionStorage.removeItem(STORAGE_KEYS.USER);
    }
  }

  private setLoading(isLoading: boolean): void {
    const current = this.authState.value;
    this.authState.next({ ...current, isLoading });
  }

  private setError(error: string): void {
    const current = this.authState.value;
    this.authState.next({ ...current, error });
  }

  // Get dashboard URL
  getDashboardUrl(): string {
    const user = this.currentUser();
    if (!user) return '/entrar';
    return '/painel';
  }

  // Get current user safely
  getCurrentUser(): Usuario | null {
    // Ensure storage is loaded in browser
    if (isPlatformBrowser(this.platformId) && !this.storageLoaded) {
      this.loadUserFromStorage();
    }
    return this.currentUser();
  }

  // Update current user in memory and storage
  updateCurrentUser(user: Usuario): void {
    this.currentUser.set(user);
    
    if (isPlatformBrowser(this.platformId)) {
      // Update in localStorage
      if (localStorage.getItem(STORAGE_KEYS.USER)) {
        localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
      }
      
      // Update in sessionStorage
      if (sessionStorage.getItem(STORAGE_KEYS.USER)) {
        sessionStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
      }
    }
  }

  // Refetch user data from server
  refetchCurrentUser(): Observable<Usuario> {
    const currentUserId = this.currentUser()?.id;
    if (!currentUserId) {
      console.warn('[AuthService] No user ID available for refetch');
      return of(this.currentUser()!);
    }
    
    console.log('[AuthService] refetchCurrentUser: Iniciando refetch para userId:', currentUserId);
    
    return this.http.get<Usuario>(`${environment.apiUrl}/usuarios/${currentUserId}`).pipe(
      tap(user => {
        console.log('[AuthService] refetchCurrentUser: Dados atualizados recebidos com sucesso');
        this.updateCurrentUser(user);
      }),
      catchError(error => {
        // Se falhar o refetch (401, 404, timeout, etc), NÃO quebrar a sessão
        // O token já está autenticado (foi restaurado do localStorage)
        // Apenas falha ao buscar dados atualizados
        console.warn('[AuthService] refetchCurrentUser: Erro ao buscar dados atualizados (status:', error.status, ')');
        console.warn('[AuthService] refetchCurrentUser: Usando dados cached do localStorage/sessionStorage');
        
        // Return cached current user - sessão permanece ativa
        const cachedUser = this.currentUser();
        if (cachedUser) {
          return of(cachedUser);
        }
        
        // Se nem user em cache, retornar erro (bem raro)
        return throwError(() => error);
      })
    );
  }

  // Change Password
  changePassword(senhaAtual: string, novaSenha: string, confirmarSenha: string): Observable<any> {
    this.setLoading(true);
    
    const request = {
      senhaAtual,
      novaSenha,
      confirmarSenha
    };

    return this.http.post(AUTH_ENDPOINTS.CHANGE_PASSWORD, request).pipe(
      tap(() => {
        this.setLoading(false);
      }),
      catchError(error => {
        this.setError(error.error?.mensagem || error.error?.message || 'Erro ao trocar senha');
        this.setLoading(false);
        throw error;
      })
    );
  }

  // Check Email Availability
  checkEmailAvailability(email: string): Observable<{ available: boolean }> {
    return this.http.get<{ available: boolean }>(`${AUTH_ENDPOINTS.CHECK_EMAIL}/${encodeURIComponent(email)}`).pipe(
      catchError(() => of({ available: true }))
    );
  }

  // Check CPF Availability
  checkCpfAvailability(cpf: string): Observable<{ available: boolean }> {
    return this.http.get<{ available: boolean }>(`${AUTH_ENDPOINTS.CHECK_CPF}/${encodeURIComponent(cpf)}`).pipe(
      catchError(() => of({ available: true }))
    );
  }

  // Check Phone Availability
  checkPhoneAvailability(phone: string): Observable<{ available: boolean }> {
    return this.http.get<{ available: boolean }>(`${AUTH_ENDPOINTS.CHECK_PHONE}/${encodeURIComponent(phone)}`).pipe(
      catchError(() => of({ available: true }))
    );
  }
}
