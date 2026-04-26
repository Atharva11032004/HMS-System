module.exports = [
  {
    context: ['/api/identity'],
    target: 'http://localhost:5220',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/identity': '' },
    logLevel: 'info'
  },
  {
    context: ['/api/reservation'],
    target: 'http://localhost:5092',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/reservation': '' },
    logLevel: 'info'
  },
  {
    context: ['/api/guest'],
    target: 'http://localhost:5281',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/guest': '' },
    logLevel: 'info'
  },
  {
    context: ['/api/room'],
    target: 'http://localhost:5187',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/room': '' },
    logLevel: 'info'
  },
  {
    context: ['/api/pricing'],
    target: 'http://localhost:5032',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/pricing': '' },
    logLevel: 'info'
  },
  {
    context: ['/api/billing'],
    target: 'http://localhost:5114',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/billing': '' },
    logLevel: 'info'
  },
  {
    context: ['/api/staff', '/api/departments'],
    target: 'http://localhost:5183',
    secure: false,
    changeOrigin: true,
    pathRewrite: { '^/api/staff': '', '^/api/departments': '' },
    logLevel: 'info'
  }
];
