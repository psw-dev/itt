pipeline {
    environment {
        IMAGE_REPO_NAME = "dev.docker.registry:5000/${(env.JOB_NAME.tokenize('/') as String[])[0]}"
    }
    agent { label 'linux' }

    stages {
        stage('Dotnet Restore and Publish') {
            steps {
                sh "dotnet clean"
                sh "dotnet restore"
                sh "dotnet publish -c Release -o out"
            }
        }
        stage('Docker Build') {
            environment {
                BUILD_IMAGE_REPO_TAG = "${env.IMAGE_REPO_NAME}:${env.BRANCH_NAME}-latest"
            }
            steps {
                sh "docker build -f Dockerfile.Jenkins . -t $BUILD_IMAGE_REPO_TAG --network=host"
                sh "docker tag $BUILD_IMAGE_REPO_TAG ${env.IMAGE_REPO_NAME}:${env.BRANCH_NAME}_$BUILD_NUMBER"
            }
        }
        stage('Docker Push') {
            environment {
                BUILD_IMAGE_REPO_TAG = "${env.IMAGE_REPO_NAME}:${env.BRANCH_NAME}-latest"
            }
            steps {
                    sh "docker push $BUILD_IMAGE_REPO_TAG"
                    sh "docker push ${env.IMAGE_REPO_NAME}:${env.BRANCH_NAME}_$BUILD_NUMBER"
            }
        }
    }
    post {
        always {
            echo "This will always run. Can send email here"
        }
    }
}