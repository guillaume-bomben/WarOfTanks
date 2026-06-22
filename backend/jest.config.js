export default {
  testEnvironment: "node",
  // Jest a besoin de cette transform vide pour gerer les ES Modules (import/export)
  transform: {},
  // Fichier execute avant/apres les tests : demarre la DB en memoire
  setupFilesAfterEnv: ["<rootDir>/src/tests/setup.js"],
  // Ou trouver les tests
  testMatch: ["<rootDir>/src/tests/**/*.test.js"],
  forceExit: true,
  clearMocks: true
};