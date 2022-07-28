namespace webIEA.Contracts
{
    public interface IRepositoryWrapper
    {
       IMemberManager MemberManager { get; }
       IMemberStatusManager MemberStatusManager { get; } 
        IMemberSpecialization MemberSpecialization { get; }
    }
}
