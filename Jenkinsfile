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
        stage ('Artifacts') {
            steps {
                archiveArtifacts artifacts: '$WORKSPACE/bin/Debug/netstandard2.0/*.dll'
            }
        }
    }
}