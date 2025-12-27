import { PerfilPaciente, PerfilProfissional } from '@core/services/users.service';

export interface Usuario {
  id: string;
  email: string;
  nome: string;
  sobrenome: string;
  cpf: string;
  telefone?: string;
  avatar?: string;
  tipo: TipoUsuario;
  status: 'Ativo' | 'Inativo';
  emailVerificado?: boolean;
  criadoEm: string;
  atualizadoEm?: string;
  
  // Perfis específicos por tipo de usuário
  perfilPaciente?: PerfilPaciente;
  perfilProfissional?: PerfilProfissional;
}

// Alias para compatibilidade
export type User = Usuario;

export type TipoUsuario = 'Paciente' | 'Profissional' | 'Administrador';
export type userrole = TipoUsuario; // Alias para compatibilidade

export interface LoginRequest {
  email: string;
  senha: string;
  lembrarMe?: boolean;
}

export interface LoginResponse {
  usuario: Usuario;
  accessToken: string;
  refreshToken: string;
}

export interface RegisterRequest {
  nome: string;
  sobrenome: string;
  email: string;
  cpf: string;
  telefone?: string;
  senha: string;
  confirmarSenha: string;
  aceitarTermos: boolean;
  tokenConvite?: string;
}

export interface RegisterResponse {
  usuario: Usuario;
  mensagem: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ForgotPasswordResponse {
  mensagem: string;
}

export interface ResetPasswordRequest {
  token: string;
  novaSenha: string;
  confirmarSenha: string;
}

export interface ResetPasswordResponse {
  mensagem: string;
}

export interface VerifyEmailRequest {
  token: string;
}

export interface VerifyEmailResponse {
  mensagem: string;
  usuario?: Usuario;
}

export interface ChangePasswordRequest {
  senhaAtual: string;
  novaSenha: string;
  confirmarSenha: string;
}

export interface ChangePasswordResponse {
  mensagem: string;
}

export interface AuthState {
  user: Usuario | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}
