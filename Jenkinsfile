#!groovy
def SendJira(branch, com){
def environmentId = branch;
def environmentType = "unmapped";
def environmentName = "null";
switch(branch) {
  case "origin/development":
      environmentId = "development-customer.forceget.com";
      environmentType = "development";
      environmentName = "development-server";
    break
  case "origin/test":
      environmentId = "test-customer.forceget.com";
      environmentType = "testing";
      environmentName = "test-server";
    break
  case "origin/main":
      environmentId = "main-customer.forceget.com";
      environmentType = "production";
      environmentName = "prod-server";
    break
}

if (com == 'build') {
          jiraSendBuildInfo() 
} else {
          jiraSendDeploymentInfo environmentId: environmentId, environmentName: environmentName, environmentType: environmentType
}
}
pipeline {
  agent none
  stages {
    stage('N-Tier - Docker Build') {
      agent any
      steps {
          sh 'docker build --build-arg BUILD_NUMBER=1.0.0.${BUILD_NUMBER} -t devops-docker.forceget.com/api/N-Tier/${GIT_BRANCH} .'
      }
      post {
        always {
          SendJira(GIT_BRANCH, 'build')
        }
      }
    }
    stage('N-Tier - Docker Push') {
      agent any
      steps {
          sh 'docker push devops-docker.forceget.com/api/N-Tier/${GIT_BRANCH}'
      }
      post {
        always {
          SendJira(GIT_BRANCH, 'deploy')
        }
      }
    }
  }
}
