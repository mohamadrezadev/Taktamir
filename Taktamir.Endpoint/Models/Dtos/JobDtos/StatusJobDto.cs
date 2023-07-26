namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public enum StatusJobDto
    {
  
        /// <summary>
        /// کنسل شده
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// درحال انجام 
        /// </summary>
        Doing = 3,
        /// <summary>
        /// حمل به کارگاه 
        /// </summary>
        Carry_off = 4,
        /// <summary>
        /// کار زمان بالا 
        /// </summary>
        High_time = 5,
        /// <summary>
        /// در انتظار برای رزرو شدن توسط تکنسین 
        /// </summary>
        waiting = 6,
    }
}
