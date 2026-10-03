package com.incleanhome.mobile

import com.incleanhome.mobile.iam.data.AuthResponse
import com.incleanhome.mobile.iam.data.LoginNextStep
import com.incleanhome.mobile.iam.data.LoginResult
import com.incleanhome.mobile.iam.data.interpretAuthResponse
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Test

class LoginViewModelTest {

    @Test
    fun testInterpretAuthResponse_RequiereConfigurar2FA() {
        //Arrange
        val response = AuthResponse(
            requires2faSetup = true,
            challengeToken = "token123"
        )

        //Act
        val resultado = interpretAuthResponse(response)

        //Assert
        assertTrue(resultado is LoginResult.Challenge)
        resultado as LoginResult.Challenge
        assertEquals(LoginNextStep.TWO_FA_SETUP, resultado.nextStep)
        assertEquals("token123", resultado.challengeToken)
    }
}