// TeleCuidar - Configuração de Interface Customizada do Jitsi
// Este arquivo sobrescreve as configurações padrão de interface

interfaceConfig = {
    // Desabilitar todas as watermarks e logos
    SHOW_JITSI_WATERMARK: false,
    SHOW_WATERMARK_FOR_GUESTS: false,
    SHOW_BRAND_WATERMARK: false,
    BRAND_WATERMARK_LINK: '',
    JITSI_WATERMARK_LINK: '',
    
    // Desabilitar logos
    DEFAULT_LOGO_URL: '',
    DEFAULT_WELCOME_PAGE_LOGO_URL: '',
    
    // Desabilitar promoções e deep linking
    SHOW_POWERED_BY: false,
    SHOW_PROMOTIONAL_CLOSE_PAGE: false,
    MOBILE_APP_PROMO: false,
    HIDE_DEEP_LINKING_LOGO: true,
    
    // Outras configurações
    DISABLE_JOIN_LEAVE_NOTIFICATIONS: false,
    HIDE_INVITE_MORE_HEADER: true,
    GENERATE_ROOMNAMES_ON_WELCOME_PAGE: false,
    LANG_DETECTION: false,
    
    // Filmstrip
    VERTICAL_FILMSTRIP: true,
    TILE_VIEW_MAX_COLUMNS: 2,
    
    // Botões da toolbar
    TOOLBAR_BUTTONS: [
        'microphone',
        'camera',
        'desktop',
        'fullscreen',
        'fodeviceselection',
        'hangup',
        'chat',
        'settings',
        'videoquality',
        'filmstrip',
        'tileview',
        'select-background',
        'mute-everyone',
        'mute-video-everyone'
    ],
    
    SETTINGS_SECTIONS: ['devices', 'language', 'moderator', 'profile'],
    DEFAULT_LOCAL_DISPLAY_NAME: 'Eu',
    DEFAULT_REMOTE_DISPLAY_NAME: 'Participante'
};
