pipeline {
    agent {label 'azcli'}
    stages {
        stage ('SCM') {
            steps {
                checkout scm
            }
        }
        stage ('Build') {
            steps {
                script {
                    sh 'dotnet restore'
                    sh 'dotnet build'
                }
            }
        }
    }
}